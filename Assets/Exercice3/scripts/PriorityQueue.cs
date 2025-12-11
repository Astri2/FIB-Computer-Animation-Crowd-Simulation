using System;
using System.Collections.Generic;
public class PriorityQueue<T>
{
    private List<T> elements;
    private readonly IComparer<T> comparer;
    // Constructor that sets up the priority queue with a specific comparer
    public PriorityQueue(IComparer<T> comparer)
    {
        this.elements = new List<T>();
        this.comparer = comparer;
    }
    // Property to get the number of elements in the queue
    public int Count => elements.Count;
    // Method to add an element to the priority queue
    public void Enqueue(T item)
    {
        elements.Add(item);
        int index = Count - 1;
        // Bubble up the newly added item to maintain heap property
        while (index > 0)
        {
            int parentIndex = (index - 1) / 2;
            if (comparer.Compare(elements[parentIndex], elements[index]) <= 0)
                break;
            Swap(index, parentIndex);
            index = parentIndex;
        }
    }
    // Method to remove and return the element with the highest priority
    public T Dequeue()
    {
        if (Count == 0)
            throw new InvalidOperationException("Queue is empty.");
        T front = elements[0];
        elements[0] = elements[Count - 1];
        elements.RemoveAt(Count - 1);
        // Push down the root element to maintain heap property
        int index = 0;
        while (true)
        {
            int leftChild = 2 * index + 1;
            if (leftChild >= Count)
                break;
            int rightChild = leftChild + 1;
            int minChild = (rightChild < Count && comparer.Compare(elements[rightChild], elements[leftChild]) < 0)
                ? rightChild
                : leftChild;
            if (comparer.Compare(elements[index], elements[minChild]) <= 0)
                break;
            Swap(index, minChild);
            index = minChild;
        }
        return front;
    }
    // Helper method to swap elements in the list
    private void Swap(int i, int j)
    {
        T temp = elements[i];
        elements[i] = elements[j];
        elements[j] = temp;
    }
}