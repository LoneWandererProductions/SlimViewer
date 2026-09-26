/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViews
 * FILE:        LayersPanel.xaml.cs
 * PURPOSE:     Sidebar panel listing the layers of a LayerDocumentController (Advanced Edit Mode's Layers
 *              view). Every mutation - add, duplicate, delete, reorder, rename, visibility, opacity, blend,
 *              active-layer selection - goes straight through LayerDocumentController, so undo/redo, the
 *              canvas (Common.Images.ImageZoom.LayeredDocument) and this panel all stay in lockstep without
 *              the panel owning any editable state of its own; it only owns LayerRowViewModel wrappers used
 *              purely for binding, rebuilt/refreshed from the controller's own change notifications.
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using Imaging.Objects.Documents;
using SlimViews.DataObjects;

namespace SlimViews
{
    /// <summary>
    ///     Sidebar UI for <see cref="LayerDocumentController" />. Follows the same "toggleable panel bound
    ///     from MainWindow's SidebarContainer" pattern as Common.Images.CifChannelEditor/FilterConfigControl/
    ///     TextureConfigControl, but lives in SlimViews rather than Common.Images because it binds directly
    ///     to <see cref="LayerDocumentController" />, which Common.Images cannot reference without creating a
    ///     circular project reference (SlimViews -&gt; SlimControls -&gt; Common.Images already).
    /// </summary>
    public sealed partial class LayersPanel : UserControl
    {
        /// <summary>Identifies the <see cref="Controller" /> dependency property.</summary>
        public static readonly DependencyProperty ControllerProperty = DependencyProperty.Register(
            nameof(Controller), typeof(LayerDocumentController), typeof(LayersPanel),
            new PropertyMetadata(null, OnControllerChanged));

        /// <summary>Identifies the <see cref="EnableLayersCommand" /> dependency property.</summary>
        public static readonly DependencyProperty EnableLayersCommandProperty = DependencyProperty.Register(
            nameof(EnableLayersCommand), typeof(ICommand), typeof(LayersPanel), new PropertyMetadata(null));

        /// <summary>
        ///     Guards <see cref="LayerList_SelectionChanged" /> while the code here is itself the one setting
        ///     <see cref="ListBox.SelectedItem" /> (from <see cref="RebuildRows" />/<see cref="OnActiveLayerChanged" />),
        ///     so that re-entrant call does not turn straight back around and call
        ///     <see cref="LayerDocumentController.SetActiveLayer" /> again.
        /// </summary>
        private bool _syncingSelection;

        /// <summary>Initializes a new instance of the <see cref="LayersPanel" /> class.</summary>
        public LayersPanel()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Gets or sets the layered document to show/edit, or null when the current image is still a
        ///     single flat bitmap (see <see cref="ImageView.Layers" />/<see cref="ImageView.EnableLayers" />).
        ///     The panel shows an "Enable Layers" prompt instead of the layer list while this is null.
        /// </summary>
        public LayerDocumentController? Controller
        {
            get => (LayerDocumentController?)GetValue(ControllerProperty);
            set => SetValue(ControllerProperty, value);
        }

        /// <summary>
        ///     Gets or sets the command that turns the current single bitmap into a layered document (bind
        ///     to <c>Commands.EnableLayers</c>). Left null, the "Enable Layers" button in the empty state
        ///     simply does nothing.
        /// </summary>
        public ICommand? EnableLayersCommand
        {
            get => (ICommand?)GetValue(EnableLayersCommandProperty);
            set => SetValue(EnableLayersCommandProperty, value);
        }

        /// <summary>Gets the rows currently shown, top of the layer stack first.</summary>
        public ObservableCollection<LayerRowViewModel> Rows { get; } = new();

        /// <summary>Gets the blend modes offered by the per-row picker.</summary>
        public BlendMode[] BlendModes { get; } = (BlendMode[])Enum.GetValues(typeof(BlendMode));

        /// <summary>Swaps the Document/ActiveLayerChanged subscriptions over to the new controller and rebuilds the rows.</summary>
        private static void OnControllerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var panel = (LayersPanel)d;

            if (e.OldValue is LayerDocumentController oldController)
            {
                oldController.Document.Changed -= panel.OnDocumentChanged;
                oldController.ActiveLayerChanged -= panel.OnActiveLayerChanged;
            }

            if (e.NewValue is LayerDocumentController newController)
            {
                newController.Document.Changed += panel.OnDocumentChanged;
                newController.ActiveLayerChanged += panel.OnActiveLayerChanged;
            }

            panel.RebuildRows();
        }

        /// <summary>
        ///     Reacts to every document edit. Structural changes (layer added/removed/reordered) rebuild the
        ///     whole row list, since row position/identity depend on the layer order; everything else updates
        ///     just the one affected row in place, so an in-progress stroke does not tear down and rebuild
        ///     rows the user might currently be editing (e.g. mid-rename) elsewhere in the stack.
        /// </summary>
        private void OnDocumentChanged(object? sender, DocumentChangedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                switch (e.Kind)
                {
                    case DocumentChangeKind.LayerAdded:
                    case DocumentChangeKind.LayerRemoved:
                    case DocumentChangeKind.LayerMoved:
                        RebuildRows();
                        break;
                    case DocumentChangeKind.LayerPropertiesChanged:
                        FindRow(e.LayerId)?.RefreshFromLayer();
                        break;
                    case DocumentChangeKind.PixelsChanged:
                    case DocumentChangeKind.ShapesChanged:
                        FindRow(e.LayerId)?.RefreshThumbnail();
                        break;
                }
            }));
        }

        /// <summary>Keeps every row's <see cref="LayerRowViewModel.IsActive" /> highlight and the ListBox selection in sync.</summary>
        private void OnActiveLayerChanged(object? sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                foreach (var row in Rows) row.RefreshIsActive();
                SyncSelectionToActiveLayer();
            }));
        }

        /// <summary>Rebuilds <see cref="Rows" /> from scratch, top of the stack (<c>Document.Layers[^1]</c>) first.</summary>
        private void RebuildRows()
        {
            Rows.Clear();

            if (Controller is null) return;

            for (var i = Controller.Document.Layers.Count - 1; i >= 0; i--)
            {
                Rows.Add(new LayerRowViewModel(Controller, Controller.Document.Layers[i]));
            }

            SyncSelectionToActiveLayer();
        }

        private LayerRowViewModel? FindRow(Guid layerId) => Rows.FirstOrDefault(r => r.Id == layerId);

        private void SyncSelectionToActiveLayer()
        {
            _syncingSelection = true;
            try
            {
                LayerList.SelectedItem = Controller?.ActiveLayerId is { } id ? FindRow(id) : null;
            }
            finally
            {
                _syncingSelection = false;
            }
        }

        /// <summary>Clicking a row makes it the active layer - the one new strokes/shapes are added to.</summary>
        private void LayerList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_syncingSelection) return;

            if (LayerList.SelectedItem is LayerRowViewModel row) Controller?.SetActiveLayer(row.Id);
        }

        /// <summary>
        ///     Commits the opacity slider's value once the drag ends (mouse up), rather than once per tick -
        ///     see <see cref="LayerRowViewModel.Opacity" />'s remarks on why. <see cref="Slider.Value" /> is
        ///     bound with <c>UpdateSourceTrigger=Explicit</c> for exactly this reason; this is what performs
        ///     the otherwise-suppressed update.
        /// </summary>
        private void OpacitySlider_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is not Slider slider) return;

            var expression = BindingOperations.GetBindingExpression(slider, RangeBase.ValueProperty);
            expression?.UpdateSource();
        }

        private LayerRowViewModel? SelectedRow => LayerList.SelectedItem as LayerRowViewModel;

        private void AddRasterLayer_Click(object sender, RoutedEventArgs e) => Controller?.AddRasterLayer();

        private void AddShapeLayer_Click(object sender, RoutedEventArgs e) => Controller?.AddShapeLayer();

        private void DuplicateLayer_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedRow is { } row) Controller?.DuplicateLayer(row.Id);
        }

        private void RemoveLayer_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedRow is { } row) Controller?.RemoveLayer(row.Id);
        }

        /// <summary>
        ///     "Up" moves a layer towards the top/front of the stack - i.e. towards the end of
        ///     <see cref="Document.Layers" /> (index 0 is the bottom; see that property's own remarks),
        ///     which is the opposite direction from "up" in the on-screen list, since the list shows the top
        ///     of the stack first. That inversion is deliberate and confined to these two handlers.
        /// </summary>
        private void MoveLayerUp_Click(object sender, RoutedEventArgs e) => MoveSelected(+1);

        private void MoveLayerDown_Click(object sender, RoutedEventArgs e) => MoveSelected(-1);

        private void MoveSelected(int delta)
        {
            if (Controller is not { } controller || SelectedRow is not { } row) return;

            var index = controller.Document.IndexOf(row.Id);
            if (index < 0) return;

            var newIndex = Math.Clamp(index + delta, 0, controller.Document.Layers.Count - 1);
            if (newIndex != index) controller.MoveLayer(row.Id, newIndex);
        }
    }
}
