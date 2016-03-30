using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace CheckAsm.Licensing.Security
{
    public class SecureStorage
    {
        private readonly string _rootPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        private readonly string _crcStorePath;

        public int BufferSize = 1024;
        public string CrcFileName = "crc.sec.amb";

        Dictionary<string,string> _crcStore;

        public SecureStorage()
        {
            _crcStorePath = Path.Combine(_rootPath, CrcFileName);
            LoadCrcStore();
        }

        public SecureStorage(string crcFileName)
        {
            CrcFileName = crcFileName;
            _crcStorePath = Path.Combine(_rootPath, CrcFileName);
            LoadCrcStore();
        }

        private void LoadCrcStore()
        {
            if(!File.Exists(_crcStorePath))
            {
                _crcStore = new Dictionary<string,string>();
                SaveCrcStore();
                return;
            }
            using(var fs = new FileStream(_crcStorePath, FileMode.Open))
            {
                var serializer = new BinaryFormatter();
                _crcStore = (Dictionary<string,string>)serializer.Deserialize(fs);
            }
            
        }

        private void SaveCrcStore()
        {
            using (var fs = new FileStream(_crcStorePath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                var serializer = new BinaryFormatter();
                serializer.Serialize(fs, _crcStore);
            }
        }

        public void Reset()
        {
            foreach(var k in _crcStore.Keys)
            {
                File.Delete(GetFullPath(k));
            }
            _crcStore.Clear();
            SaveCrcStore();
        }

        public string GetFullPath(string fileName)
        {
            return Path.Combine(_rootPath, fileName);
        }

        public void SaveFile(string fileName, byte[] content)
        {
            var path = Path.Combine(_rootPath,fileName);
            using (var fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
            {
                using (var writer = new BinaryWriter(fs))
                {
                    writer.Write(content);
                }
            }
            if (_crcStore.ContainsKey(fileName))
            {
                _crcStore[fileName] = GetHash(content);
            }
            else
            {
                _crcStore.Add(fileName, GetHash(content));
            }
            SaveCrcStore();
        }

        public string GetHash(byte[] bytes)
        {
            byte[] hash = MD5.Create().ComputeHash(bytes);
           
            var sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }

        public byte[] LoadFile(string fileName)
        {
            var path = Path.Combine(_rootPath, fileName);
            byte[] content;
            if (File.Exists(path))
            {
                content = new byte[new FileInfo(path).Length];
            }
            else throw new FileNotFoundException();

            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                using (var reader = new BinaryReader(fs))
                {
                    byte[] buffer = new byte[BufferSize];
                    int index = 0;
                    int bytesRead = 0;
                    do
                    {
                        bytesRead = reader.Read(buffer, 0, buffer.Length);
                        if (bytesRead == buffer.Length)
                        {
                            buffer.CopyTo(content, index);
                        }
                        else
                        {
                            for (int i = 0; i < bytesRead; i++)
                            {
                                content[index + i] = buffer[i];
                            }
                        }
                        index += bytesRead;
                    } while (bytesRead > 0);
                }
            }

            if (!_crcStore.ContainsKey(fileName))
            {
                throw new FileNotFoundException();
            }

            if (_crcStore[fileName] != GetHash(content))
            {
                throw new SecurityException("File was modified");
            }

            return content;
        }
    }
}
