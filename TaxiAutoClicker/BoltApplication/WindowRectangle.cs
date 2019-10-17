using System.Drawing;

namespace TaxiAutoClicker.BoltApplication
{
    class WindowRectangle
    {
        private readonly Point _leftTop;
        private readonly int _width;
        private readonly int _height;

        public WindowRectangle(Point leftTop, int width, int height)
        {
            _leftTop = leftTop;
            _width = width;
            _height = height;
        }

        /// <summary>
        /// По относительному положению координат точки в окне
        /// вычисляет абсолютные координаты данной точки относительно
        /// левого верхнего угла экрана всего монитора.
        /// </summary>
        /// <param name="relativePoint">Относительное положение точки в окне.</param>
        /// <returns>Абсолютные координаты точки на экране.</returns>
        public Point GetScreenPoint(PointF relativePoint)
        {
            var x = (int)(_width * relativePoint.X) + _leftTop.X;
            var y = (int)(_height * relativePoint.Y) + _leftTop.Y;
            return new Point(x, y);
        }
    }
}
