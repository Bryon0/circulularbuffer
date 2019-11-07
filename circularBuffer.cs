using System;
using System.Collections.Generic;
using System.Text;

sealed class CircularBuffer<T>
{      
    private uint tail;
    private uint head;
    private uint numMessages;
    private uint size;
    private uint sizeMessage;

    private T[] buffer;
    private T[,] dbuffer;

    private readonly object countLock = new object();
    /*
    * 
    * Constructors  
    * 
    */

    /// <summary>
    /// 
    /// </summary>
    /// <param name="xSize"></param>
    public CircularBuffer(uint xSize)
    {
        buffer = new T[xSize];
        tail = 0;
        head = 0;
        size = xSize;
        numMessages = 0;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="xSize"></param>
    /// <param name="ySize"></param>
    public CircularBuffer(uint xSize, uint ySize)
    {
        dbuffer = new T[xSize, ySize];
        tail = 0;
        head = 0;
        size = xSize;
        sizeMessage = ySize;
        numMessages = 0;
    }

    /*
        * 
        *  Methods
        * 
        */ 

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public int Push(T t)
    {
        buffer[head] = t;
        lock (countLock)
        {
            numMessages += 1;
        }
        head = (head + 1) % size;
            
        return 1;
    }

    /// <summary>
    /// Looks at the most recent data inserted into the array.
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public int Peek(ref T t)
    {
        if (IsEmpty())
        {
            return 0;
        }

        t = buffer[head];
           
        return 1;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public int Pop(ref T t)
    {
        if (IsEmpty())
        {
            return 0;
        }

        t = buffer[tail];

        lock (countLock)
        {
            if ((numMessages -= numMessages) < 0)
            {
                numMessages = 0;
            }
        }

        tail = (tail + 1) % size;
            
        return 1;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    public int Push(T[] data, uint length)
    {
        for(uint x = 0; x < length; x++)
        {
            if(x > size)
            {
                return -1;
            }
            else
            {
                dbuffer[head, x] = data[x];
            }
        }

        lock (countLock)
        {
            numMessages += 1;
        }

        head = (head + 1) % size;
            
        return 1;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    public int Pop(ref T[] data, uint length)
    {
        if (IsEmpty())
        {
            return 0;
        }

        for (uint x = 0; x < length; x++)
        {
            if (x > sizeMessage)
            {
                return -1;
            }
            else
            {
                data[x] = dbuffer[head, x];
            }
        }

        lock (countLock)
        {
            if ((numMessages -= numMessages) < 0)
            {
                numMessages = 0;
            }
        }

        tail = (tail + 1) % size;
           
        return 1;
    }

    /// <summary>
    /// //Check if the buffer is empty. 
    /// </summary>
    /// <returns></returns>
    public Boolean IsEmpty()
    {
        uint num = 0;

        lock(countLock)
        {
            num = numMessages;
        }
        return ((head == tail) && (0 == num));
    }

    /// <summary>
    /// //Reset the circular buffer.
    /// </summary>
    public void Reset()
    {
        tail = 0;
        head = 0;
        numMessages = 0;
    }

    /// <summary>
    /// Get the number of mesages in the buffer.
    /// </summary>
    /// <returns></returns>
    public uint NumberOFMessagesReceived()
    {
        uint num = 0;
        lock(countLock)
        {
            num = numMessages;
        }
        return num;
    }
}

