namespace Cawotte.Toolbox
{

    using System;

    /// <summary>
    /// It's a 2D Array, but Serializable by the Unity inspector as a single dim array, because it's 
    /// a single dim array used as a 2D array.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class Serializable2DArray<T>
    {
        public int Width;
        public int Height;
        public T[] Array;

        public T this[int i, int j]
        {
            get
            {
                return Array[(j * Width) + i];
            }
            set
            {
                Array[(j * Width) + i] = value;
            }
        }

        public Serializable2DArray(int width, int height)
        {
            Width = width;
            Height = height;
            Array = new T[width * height];
        }
    }

}


