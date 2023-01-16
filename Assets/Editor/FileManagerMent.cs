using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace CreateScriptable
{
    public class FileManageMent
    {
        private ReadExcel rf = null;
        private WriteExcel wf = null;
        private string writePath = null;
        private string GUID = null;
        private string fileName = null;

        public FileManageMent(string readPath, string writePath, string GUID, string fileName)
        {
            rf = new ReadExcel(readPath);
            this.writePath = writePath;
            this.GUID = GUID;
            this.fileName = fileName;
        }

        public void Run()
        {
            
            if (rf.Run() == true)
            {
                Debug.Log("Excel File Read And Copy Complete...");
                wf = new WriteExcel(rf.read, writePath, GUID, fileName);
                if (wf.Run() == true)
                {
                    Debug.Log("Complete...");
                    return;
                }
            }
            else
            {
                Debug.Log("Excel File Read And Copy Failed...");
                return;
            }
        }
    }
}
