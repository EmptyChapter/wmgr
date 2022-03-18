namespace WindowManager
{
    internal struct Int32Size
    {
        public int _width;
        public int _height;

        public int Width
        {
            get => _width;
            set => _width = value;
        }

        public int Height
        {
            get => _height;
            set => _height = value;
        }

        public Int32Size(int width, int height) : this()
        {
            _width =    width;
            _height =   height;
        }

        public Int32Size(double width, double height) : this()
        {
            _width =    (int)width;
            _height =   (int)height;
        }
    }
}
