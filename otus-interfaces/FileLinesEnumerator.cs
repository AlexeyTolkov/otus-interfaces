using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;

namespace otus_interfaces
{
    public class FileLinesEnumerator : IEnumerator<string>
    {
        private readonly StreamReader _streamReader;


        public FileLinesEnumerator(string fileName)
        {
            _streamReader = new StreamReader(new FileStream(fileName, FileMode.Open));
        }

        public string Current { get; private set; }

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            this._streamReader.Close();
        }

        public bool MoveNext()
        {
            if (this._streamReader.EndOfStream)
            {
                return false;
            }

            this.Current = this._streamReader.ReadLine();

            return true;
        }

        public void Reset()
        {
        }
    }
}