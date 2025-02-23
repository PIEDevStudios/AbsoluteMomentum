using UnityEngine;

public class CircularBuffer<T>
{
    private T[] buffer;
    private int bufferSize;

    public CircularBuffer(int size)
    {
        this.bufferSize = size;
        buffer = new T[bufferSize];
    }
    
    public void Add(T item, int index ) => buffer[index % bufferSize] = item;
    public T Get(int index) => buffer[index % bufferSize];
    public void Clear() => buffer = new T[bufferSize];
}
