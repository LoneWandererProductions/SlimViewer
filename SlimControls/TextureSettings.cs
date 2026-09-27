/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimControls
 * FILE:        TextureSettings.cs
 * PURPOSE:     Settings for the Texture area mode - which texture pattern to fill with. See
 *              AreaModeSettings for what Enabled means and why it is no longer stored here.
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

namespace SlimControls
{
    /// <summary>
    /// Settings for the "🧱 Texture" area mode.
    /// </summary>
    /// <seealso cref="SlimControls.AreaModeSettings" />
    public sealed class TextureSettings : AreaModeSettings
    {
        /// <summary>
        /// The texture name
        /// </summary>
        private string? _textureName;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextureSettings" /> class.
        /// </summary>
        /// <param name="owner">The <see cref="DrawingState" /> this settings object belongs to.</param>
        public TextureSettings(DrawingState owner) : base(owner, AreaMode.Texture)
        {
        }

        /// <summary>
        /// Gets or sets the name of the selected texture pattern (see <see cref="DrawingState.Textures" />).
        /// </summary>
        /// <value>
        /// The name of the texture.
        /// </value>
        public string? TextureName
        {
            get => _textureName;
            set
            {
                if (_textureName == value) return;

                _textureName = value;
                OnPropertyChanged(nameof(TextureName));
            }
        }
    }
}