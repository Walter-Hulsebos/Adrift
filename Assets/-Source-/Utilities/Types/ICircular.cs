using JetBrains.Annotations;

namespace Utilities.Types
{
    public interface ICircularCollection<T>
    {
        /// <summary>
        /// Gets the <see cref="ICircularCollection{T}"/> at the specified index.
        /// </summary>
        /// 
        /// <param name="index"> Index. </param>
        [PublicAPI]
        T this[int index]
        {
            get;
            set;
        }
       
        /*
        ArrayIndexInfo Min();
        ArrayIndexInfo Max();


        /// <summary>
        /// Gets Average of the <see cref="ICircularCollection"/>
        /// </summary>
        /// <returns> The average. </returns>
        [PublicAPI]
        T Average { get; }
        */

        /// <summary>
        /// Insert the specified value.
        /// </summary>
        /// <param name="value">Value.</param>
        [PublicAPI]
        void Insert(in T value);

        /// <summary>
        /// Removes at index.
        /// </summary>
        /// <param name="index">Index.</param>
        [PublicAPI]
        void RemoveAt(in int index);

        /// <summary>
        /// Clear this instance.
        /// </summary>
        void Clear();

        //void Print();
    }
}