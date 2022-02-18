using System;

namespace Simple2D_AStar.Internal
{
    internal struct Heap<T> where T : IComparable<T>
    {
        #region Data
        private readonly T[] collection;
        #endregion

        #region Properties
        public int Count { get; private set; }
        public int Capacity => collection.Length;
        #endregion

        #region Constructor
        public Heap(int capacity)
        {
            collection = new T[capacity];
            Count = 0;
        }
        public Heap(T[] collection)
        {
            this.collection = collection;
            Count = collection.Length;
        }
        #endregion

        #region Public Functions
        public T PeakFirst() => collection[0];

        public void Add(T element)
        {
            if (Count == collection.Length)
                throw new IndexOutOfRangeException();

            collection[Count] = element;
            Count++;

            RecalculateUp();
        }
        public T RemoveFirst()
        {

            if (Count == 0)
                throw new IndexOutOfRangeException();

            T first = collection[0];

            collection[0] = collection[Count - 1];
            Count--;

            RecalculateDown();
            return first;
        }
        #endregion

        #region Private Functions
        private void SwapIndex(int firstIndex, int secondIndex)
        {
            (collection[secondIndex], collection[firstIndex]) = (collection[firstIndex], collection[secondIndex]);
        }

        private void RecalculateUp()
        {
            int itemIndex = Count - 1;

            while (itemIndex != 0) {

                int parentIndex = (itemIndex - 1) / 2;
                if (collection[itemIndex].CompareTo(collection[parentIndex]) >= 0) {
                    return;
                }

                SwapIndex(itemIndex, parentIndex);
                itemIndex = parentIndex;
            }
        }
        private void RecalculateDown()
        {
            int itemIndex = 0;

            while (itemIndex * 2 + 1 <= Count) {

                int smallIndex = itemIndex * 2 + 1;

                if (smallIndex + 1 < Count &&
                    collection[smallIndex + 1].CompareTo(collection[smallIndex]) < 0) {
                    smallIndex += 1;
                }

                if (collection[smallIndex].CompareTo(collection[itemIndex]) >= 0) {
                    return;
                }

                SwapIndex(smallIndex, itemIndex);
                itemIndex = smallIndex;
            }
        }
        #endregion
    }
}
