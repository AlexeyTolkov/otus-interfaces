using System.Collections.Generic;
using System.Collections;

namespace otus_interfaces
{
    public class FileReader : IEnumerable<string>
    {
        private readonly string _fileName;

        public FileReader(string fileName)
        {
            _fileName = fileName;
        }

        public IEnumerator<string> GetEnumerator()
        {
            return new FileLinesEnumerator(_fileName);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}