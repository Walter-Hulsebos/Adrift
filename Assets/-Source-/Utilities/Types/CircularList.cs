using System;
using JetBrains.Annotations;

//Stolen from here: https://github.com/alimsahy/CircularList

namespace Utilities.Types
{
    /// <summary>
    /// A Circular List.
    /// </summary>
    public sealed class CircularList<T> : ICircularCollection<T>
    {
        private T[] _array;
        
        public T this[int index]
        {
            get => _array[index];
            set => _array[index] = value;
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>The count.</value>
        [PublicAPI]
        public int Count => _array.Length;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:CircularList.CircularList"/> class.
        /// </summary>
        public CircularList()
        {
            _array = new T[0];
        }
        
        public void Insert(in T value)
        {
            int __length = _array.Length + 1;
            T[] __result = new T[__length];

            for (int __index = 0; __index < __length - 1; __index++)
            {
                __result[__index] = _array[__index];
            }
            __result[__length - 1] = value;
                
            _array = __result;
        }
        
        public void RemoveAt(in int index)
        {
            int __length = _array.Length - 1;
            T[] __result = new T[__length];

            if (index < __length + 1)
            {
                int __counter = 0;

                for (int __index = 0; __index < __length + 1; __index++)
                {
                    if (index == __index) continue;

                    __result[__counter] = _array[__index];
                    __counter++;
                }
                _array = __result;
            }
            else
            {
                throw new ArgumentOutOfRangeException(paramName: nameof(index), message: "Invalid index");
            }
        }

        /*
        /// <summary>
        ///     Gets the element before the minimum element.
        /// </summary>
        /// <returns>The previous.</returns>
        /// <param name="minimum">Minimum.</param>
        public ArrayIndexInfo GetPrevious(ArrayIndexInfo minimum)
        {
            return GetPreviousBeforeMinimum(minimum);
        }

        /// <summary>
        ///     Gets minimum value
        /// </summary>
        /// <returns>The minimum.</returns>
        public ArrayIndexInfo Min()
        {
            return GetMinimumValueWithIndex();
        }

        /// <summary>
        ///     Gets maximum value
        /// </summary>
        /// <returns>The max.</returns>
        public ArrayIndexInfo Max()
        {
            return GetMaximumValueWithIndex();
        }
        */
        
        public void Clear()
        {
            _array = null;
        }

        /// <summary>
        ///     Gets the array sequence.
        /// </summary>
        /// <returns>The array sequence.</returns>
        /// <param name="sequence">Sequence.</param>
        public T GetArraySequence(int sequence)
        {
            int __length = _array.Length;
            int __index = sequence - 1;
            
            T __result = default;

            if (__length >= __index)
            {
                __result = _array[__index];
            }
            else throw new ArgumentOutOfRangeException(paramName: "Invalid index");
            return __result;
        }

        /*
        /// <summary>
        ///     Gets the previous before minimum.
        /// </summary>
        /// <returns>The previous before minimum.</returns>
        /// <param name="minimum">Minimum.</param>
        protected ArrayIndexInfo GetPreviousBeforeMinimum(ArrayIndexInfo minimum)
        {
            ArrayIndexInfo previous = new ArrayIndexInfo();
            if (minimum.Index > 0)
            {
                previous.Index = minimum.Index - 1;
                previous.Value = _array[previous.Index];
            }
            else
            {
                previous.Index = _array.Length - 1;
                previous.Value = _array[previous.Index];
            }
            return previous;
        }

        /// <summary>
        ///     Gets the maximum index of the value with.
        /// </summary>
        /// <returns>The maximum value with index.</returns>
        protected ArrayIndexInfo GetMaximumValueWithIndex()
        {
            ArrayIndexInfo indexInfo = new ArrayIndexInfo();
            indexInfo.Index = -1;
            indexInfo.Value = _array[0];

            for (int i = 0; i < _array.Length; i++)
            {
                int value = _array[i];
                if (value > indexInfo.Value)
                {
                    indexInfo.Index = i;
                    indexInfo.Value = value;
                }
            }
            return indexInfo;
        }
    

        /// <summary>
        ///     Gets the minimum index of the value with.
        /// </summary>
        /// <returns>The minimum value with index.</returns>
        protected ArrayIndexInfo GetMinimumValueWithIndex()
        {
            ArrayIndexInfo indexInfo = new ArrayIndexInfo();
            indexInfo.Index = -1;
            indexInfo.Value = _array[0];

            for (int i = 0; i < _array.Length; i++)
            {
                int value = _array[i];
                if (value < indexInfo.Value)
                {
                    indexInfo.Index = i;
                    indexInfo.Value = value;
                }
            }
            return indexInfo;
        }
        */
    }
}