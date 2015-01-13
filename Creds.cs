using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClearProcess
{
    class Creds
    {
        protected string _file = "creds.txt";
        protected string _fullpath;
        protected string _line;
        protected string _id;
        protected string _pw;
        protected string _svr;
        protected string _db;

        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public string Pw
        {
            get { return _pw; }
            set { _pw = value; }
        }

        public string Server
        {
            get { return _svr; }
            set { _svr = value; }
        }

        public string Database
        {
            get { return _db; }
            set { _db = value; }
        }

        public bool IsFile
        {
            get { return File.Exists(_fullpath); }
        }

        public void Save(string id, string pw, string server, string db)
        {
            // Set values
            _id = id; _pw = pw; _svr = server; _db = db;
            // Save file
            this.Save();
        }

        public void Save()
        {
            if (!CheckVals()) { return; }
            FileStream fs = new FileStream(_fullpath, FileMode.Create);
            try
            {
                Encoder();
                byte[] buffer = ASCIIEncoding.ASCII.GetBytes(_line);
                fs.Write(buffer, 0, buffer.Length);
            }
            catch (Exception e) { throw new Exception("Saving error", e); }
            finally
            {
                fs.Close();
                fs.Dispose();
            }
        }

        public void Load()
        {
            FileStream fs = new FileStream(_fullpath, FileMode.Open);
            try
            {
                int sum = 0;
                int count;
                int length = (int)fs.Length;
                byte[] buffer = new byte[length];
                while ((count = fs.Read(buffer, sum, length - sum)) > 0) { sum += count; } // Does this have to be a loop?
                _line = ASCIIEncoding.ASCII.GetString(buffer);
                Decoder();
            }
            catch (Exception e) { throw new Exception("Loading error", e); }
            finally
            {
                fs.Close();
                fs.Dispose();
            }
        }

        public Creds()
        {
            _fullpath = Directory.GetCurrentDirectory() + "\\" + _file;
        }

        /*****
         * The Encoder and Decoder functions aren't for security so much as obscurity.
         * They exist to make a bot's quick scan for passwords more difficult.
         *****/
        protected void Encoder()
        {
            string line = _id + "|" + _pw + "|" + _svr + "|" + _db;
            byte[] bytebuffer = System.Text.UTF8Encoding.UTF8.GetBytes(line);
            _line = System.Convert.ToBase64String(bytebuffer, Base64FormattingOptions.None);
        }
        protected void Decoder()
        {
            byte[] bytebuffer = System.Convert.FromBase64String(_line);
            string line = System.Text.UTF8Encoding.UTF8.GetString(bytebuffer);
            string[] parts = line.Split(new string[] { "|" }, StringSplitOptions.None);
            _id = parts[0]; _pw = parts[1]; _svr = parts[2]; _db = parts[3];
        }

        protected bool CheckVals()
        {
            // Must have values
            if (_id.Length == 0) { return false; }
            if (_svr.Length == 0) { return false; }
            if (_db.Length == 0) { return false; }
            // Password can be blank, gets set to "nada"
            if (_pw.Length == 0) { _pw = "nada"; }
            return true;
        }
    }
}
