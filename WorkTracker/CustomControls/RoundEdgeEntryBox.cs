using Xamarin.Forms;

namespace WorkTracker.CustomControls
{
    public class RoundEdgeEntryBox : Entry
    {
        public static BindableProperty CornerRadiusProperty =
            BindableProperty.Create(nameof(CornerRadius), typeof(int), typeof(RoundEdgeEntryBox), 0);

        public static BindableProperty BorderThicknessProperty =
            BindableProperty.Create(nameof(BorderThickness), typeof(int), typeof(RoundEdgeEntryBox), 0);

        public static BindableProperty PaddingProperty =
            BindableProperty.Create(nameof(Padding), typeof(Thickness), typeof(RoundEdgeEntryBox), new Thickness(5));

        public static BindableProperty BorderColorProperty =
            BindableProperty.Create(nameof(BorderColor), typeof(Color), typeof(RoundEdgeEntryBox), Color.Transparent);

        public static readonly BindableProperty ImageProperty =
            BindableProperty.Create(nameof(Image), typeof(string), typeof(RoundEdgeEntryBox), string.Empty);

        public static readonly BindableProperty LineColorProperty =
            BindableProperty.Create(nameof(LineColor), typeof(Color), typeof(RoundEdgeEntryBox),
                Color.White);

        public static readonly BindableProperty ImageHeightProperty =
            BindableProperty.Create(nameof(ImageHeight), typeof(int), typeof(RoundEdgeEntryBox), 40);

        public static readonly BindableProperty ImageWidthProperty =
            BindableProperty.Create(nameof(ImageWidth), typeof(int), typeof(RoundEdgeEntryBox), 40);

        public static readonly BindableProperty ImageAlignmentProperty =
            BindableProperty.Create(nameof(ImageAlignment), typeof(ImageAlignment), typeof(RoundEdgeEntryBox),
                ImageAlignment.Left);

        public int CornerRadius
        {
            get => (int) GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        public int BorderThickness
        {
            get => (int) GetValue(BorderThicknessProperty);
            set => SetValue(BorderThicknessProperty, value);
        }

        public Color BorderColor
        {
            get => (Color) GetValue(BorderColorProperty);
            set => SetValue(BorderColorProperty, value);
        }

        /// <summary>
        ///     This property cannot be changed at runtime in iOS.
        /// </summary>
        public Thickness Padding
        {
            get => (Thickness) GetValue(PaddingProperty);
            set => SetValue(PaddingProperty, value);
        }

        public Color LineColor
        {
            get => (Color) GetValue(LineColorProperty);
            set => SetValue(LineColorProperty, value);
        }

        public int ImageWidth
        {
            get => (int) GetValue(ImageWidthProperty);
            set => SetValue(ImageWidthProperty, value);
        }

        public int ImageHeight
        {
            get => (int) GetValue(ImageHeightProperty);
            set => SetValue(ImageHeightProperty, value);
        }

        public string Image
        {
            get => (string) GetValue(ImageProperty);
            set => SetValue(ImageProperty, value);
        }

        public ImageAlignment ImageAlignment
        {
            get => (ImageAlignment) GetValue(ImageAlignmentProperty);
            set => SetValue(ImageAlignmentProperty, value);
        }
    }

    public enum ImageAlignment
    {
        Left,
        Right
    }
}