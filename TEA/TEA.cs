using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TEA
{
    public class Encode
    {
        readonly Byte[] _inputArray;
        readonly uint[] _key = new uint[4];
        Byte[] _outputArray;


        public Byte[] InputArray
        {
            get
            {
                return _inputArray;
            }
        }
        public Byte[] EncodeArray
        {
            get
            {
                return _outputArray;
            }
        }

        public uint[] Key
        {
            get
            {
                return _key;
            }
        }

        // конструктор для заданого ключа
        public Encode(Byte[] InputArray, uint[] Key)
        {
            _inputArray = InputArray;
            _key = Key;
        }
        // конструктор без заданого ключа

        public Encode(Byte[] InputArray)
        {
            _inputArray = InputArray;
            _key = this.GenerateKey();
        }

        private uint[] GenerateKey()
        {
            Random rnd1 = new Random();
            Random rnd2 = new Random();
            Random rnd3 = new Random();
            Random rnd4 = new Random();

            uint[] key = new uint[4];

            key[0] = Convert.ToUInt32(rnd1.Next());
            key[1] = Convert.ToUInt32(rnd2.Next());
            key[2] = Convert.ToUInt32(rnd3.Next());
            key[3] = Convert.ToUInt32(rnd4.Next());

            return key;

        }

        public void FullEncode()
        {
            uint[][] NewArray = this.ChangeArray();
            int n = _inputArray.Length / 8+1;
            for (int i = 0; i < n; i++)
            {
                Сode(NewArray[i], _key);
            }
            _outputArray = this.ReturnArrayToByte(NewArray);

        }

        private Byte[] ReturnArrayToByte(uint[][] NewArray)
        {
            int n = _inputArray.Length / 8 + 1;
            int k = _inputArray.Length % 8;
            int m = 8 - k;
            Byte[] A = new Byte[_inputArray.Length + m];
            Byte[] d=new Byte[4];

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    d = BitConverter.GetBytes(NewArray[i][j]);
                    for (int l = 0; l < 4; l++)
                    {
                        A[8 * i + 4 * j + l] = d[l];
                    }
                }
            }

            return A;
        }


        private uint[][] ChangeArray()
        {
            //int n = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(_inputArray.Length / 8)));
            int n = _inputArray.Length / 8+1;
            int t = _inputArray.Length / 8;
            Byte[] NewArr;
            if (n > t+1)
            {
                int k = _inputArray.Length % 8;
                int m = 8 - k;
                NewArr = new Byte[_inputArray.Length + m];
                for (int i = 0; i < _inputArray.Length; i++)
                {
                    NewArr[i] = _inputArray[i];
                }
                for (int i = 0; i < m-1; i++)
                {
                    NewArr[_inputArray.Length + i] = 0x01;
                }
                NewArr[_inputArray.Length + m-1] = (byte)m;
            }
            else
            {
                NewArr = new Byte[_inputArray.Length+8];
                for (int i = 0; i < _inputArray.Length; i++)
                {
                    NewArr[i] = _inputArray[i];
                }
                for (int i = 0; i < 7; i++)
                {
                    NewArr[_inputArray.Length + i] = 0x01;
                }
                NewArr[_inputArray.Length + 7] = 8;
            }
            uint[][] _32xArray = new uint[n][];

            for (int i = 0; i < n; i++)
            {
                _32xArray[i] = new uint[2];
                for (int j = 0; j < 2; j++)
                {
                    _32xArray[i][j] = BitConverter.ToUInt32(NewArr, 8 * i + 4 * j);
                }
            }

            return _32xArray;
        }

        private void Сode(uint[] v, uint[] k)
        {
            uint y = v[0]; uint z = v[1];
            uint sum = 0;
            uint delta = 0x9E3779B9; uint n = 32;
            while (n-- > 0)
            {
                y += (z << 4 ^ z >> 5) + z ^ sum + k[sum & 3];
                sum += delta;
                z += (y << 4 ^ y >> 5) + y ^ sum + k[sum >> 11 & 3];
            }
            v[0] = y; v[1] = z;
        }

    }

    public class Decode
    {
        readonly Byte[] _inputArray;
        readonly uint[] _key = new uint[4];
        Byte[] _outputArray;
        int sizeofsuperfluous;


        public Byte[] DecodeArray
        {
            get
            {
                return _outputArray;
            }
        }

        public Byte[] InputArray
        {
            get
            {
                return _inputArray;
            }
        }

        public uint[] Key
        {
            get
            {
                return _key;
            }
        }

        // конструктор для заданого ключа
        public Decode(Byte[] InputArray, uint[] Key)
        {
            _inputArray = InputArray;
            _key = Key;
            
        }


        public void FullDecode()
        {
            uint[][] NewArray = this.ChangeArray();
            int n = (int)Math.Ceiling(_inputArray.Length / 8.0);
            for (int i = 0; i < n; i++)
            {
                Сode(NewArray[i], _key);
            }
            Byte[] _tempArray = this.ReturnArrayToByte(NewArray);
            sizeofsuperfluous=_tempArray[_tempArray.Length-1];
            _outputArray=new Byte[_tempArray.Length-sizeofsuperfluous];
            for (int i = 0; i < _tempArray.Length - sizeofsuperfluous; i++)
            {
                _outputArray[i] = _tempArray[i];
            }

        }


        private Byte[] ReturnArrayToByte(uint[][] NewArray)
        {
            int n = (int)Math.Ceiling(_inputArray.Length / 8.0);
            //int k = _inputArray.Length % 8;
            //int m = 8 - k;
            Byte[] A = new Byte[_inputArray.Length];
            Byte[] d = new Byte[4];

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    d = BitConverter.GetBytes(NewArray[i][j]);
                    for (int l = 0; l < 4; l++)
                    {
                        A[8 * i + 4 * j + l] = d[l];
                    }
                }
            }

            return A;
        }


        private uint[][] ChangeArray()
        {
            int n = (int)Math.Ceiling(_inputArray.Length / 8.0);

            uint[][] _32xArray = new uint[n][];

            for (int i = 0; i < n; i++)
            {
                _32xArray[i] = new uint[2];
                for (int j = 0; j < 2; j++)
                {
                    _32xArray[i][j] = BitConverter.ToUInt32(_inputArray, 8 * i + 4 * j);
                }
            }

            return _32xArray;
        }

        private void Сode(uint[] v, uint[] k)
        {
            uint n = 32;
            uint sum;
            uint y = v[0];
            uint z = v[1];
            uint delta = 0x9e3779b9;

            sum = delta << 5;

            while (n-- > 0)
            {
                z -= (y << 4 ^ y >> 5) + y ^ sum + k[sum >> 11 & 3];
                sum -= delta;
                y -= (z << 4 ^ z >> 5) + z ^ sum + k[sum & 3];
            }

            v[0] = y;
            v[1] = z;
        }
    }
}
