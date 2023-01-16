using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace CreateScriptable
{
    internal class ReadExcel
    {
        private string path = null;
        private StreamReader sr = null;
        public List<string> read = null;

        public ReadExcel(string path)
        {
            read = new List<string>();
            this.path = path;
            FileStream excel = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            if (excel.Length<= 0)
            {
                Debug.Log(path + " is not exists");
                return;
            }
            else
            {
                Debug.Log("Checked Path : " + path + " is exists...");
                sr = new StreamReader(excel);
            }

        }

        public bool Run()
        {
            string temp = null;
            try
            {
                while ((temp = sr.ReadLine()) != null)
                {
                    read.Add(temp);
                }

            }
            catch (Exception e)
            {
                Debug.Log(e);
                sr.Close();
                return false;
            }


            sr.Close();
            return true;
        }



    }
}

