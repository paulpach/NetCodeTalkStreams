/*
The MIT License (MIT)

Copyright (c) 2020 Fredrik Holmstrom

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

using System;

namespace Transport {
  public class RingBuffer<T> {
    int _head;
    int _tail;
    int _count;

    T[] _array;

    public int  Count  => _count;
    public bool IsFull => _count == _array.Length;

    public RingBuffer(int capacity) {
      _array = new T[capacity];
    }

    public T Peek() {
      if (_count == 0) {
        throw new InvalidOperationException();
      }
      
      return _array[_tail];
    }

    public void Push(T item) {
      if (IsFull) {
        throw new InvalidOperationException();
      }

      _array[_head] =  item;
      _head         =  (_head + 1) % _array.Length;
      _count        += 1;
    }

    public T Pop() {
      if (_count == 0) {
        throw new InvalidOperationException();
      }
      
      var item = _array[_tail];

      _array[_tail] =  default;
      _tail         =  (_tail + 1) % _array.Length;
      _count        -= 1;

      return item;
    }

    public void Clear() {
      _head  = 0;
      _tail  = 0;
      _count = 0;
      
      Array.Clear(_array, 0, _array.Length);
    }
  }
}