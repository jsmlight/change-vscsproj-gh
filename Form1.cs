using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Dialogs;


namespace change_vscsproj_gh
{
    public partial class Form1 : Form
    {
        string sourceFolder = string.Empty;
        string targetFolder = string.Empty;
        string toolVersion = string.Empty;
        string netVersion = string.Empty;

        List<string> sourceFolderList;
        List<string> csprojList;

        public Form1()
        {
            InitializeComponent();

            sourceFolderList = new List<string>();
            csprojList = new List<string>();


        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (var dialog = new CommonOpenFileDialog())
            {
                dialog.IsFolderPicker = true;
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    textBox1.Text = dialog.FileName;
                }
            }

        }

        // tool version 변경
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string _inputVal = comboBox1.SelectedItem.ToString().Trim();
            _inputVal = _inputVal.Split('|')[0].ToString();
            toolVersion = _inputVal.Trim();
        }

        // .net version 변경
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            string _inputVal = comboBox2.SelectedItem.ToString().Trim();
            _inputVal = _inputVal.Split('|')[0].ToString();
            netVersion = _inputVal.Trim();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1_init();
            comboBox2_init();
        }

        public void comboBox1_init()
        {
            comboBox1.Items.Add("17.0 | VisualStudio 2022");
            comboBox1.Items.Add("16.0 | VisualStudio 2019");
            comboBox1.Items.Add("15.0 | VisualStudio 2017");
            comboBox1.Items.Add("14.0 | VisualStudio 2015");
            comboBox1.Items.Add("12.0 | VisualStudio 2013");
            comboBox1.Items.Add("11.0 | VisualStudio 2012");
            comboBox1.Items.Add("10.0 | VisualStudio 2010");
            comboBox1.Items.Add(" 9.0 | VisualStudio 2008");
            comboBox1.Items.Add(" 8.0 | VisualStudio 2005");
            comboBox1.Items.Add(" 7.0 | VisualStudio .NET(2002)");
            comboBox1.Items.Add(" 6.0 | VisualStudio 6.0");
            comboBox1.Items.Add(" 5.0 | VisualStudio 97");
        }

        public void comboBox2_init()
        {
            comboBox2.Items.Add("v4.8 | VisualStudio 2019 later");
            comboBox2.Items.Add("v4.7.2 | VisualStudio 2017 later");
            comboBox2.Items.Add("v4.7.1 | VisualStudio 2017 later");
            comboBox2.Items.Add("v4.7 | VisualStudio 2017 later");
            comboBox2.Items.Add("v4.6.2 | VisualStudio 2017 later");
            comboBox2.Items.Add("v4.6.1 | VisualStudio 2015 later");
            comboBox2.Items.Add("v4.6 | VisualStudio 2015 later");
            comboBox2.Items.Add("v4.5.1 | VisualStudio 2013 later");
            comboBox2.Items.Add("v4.5 | VisualStudio 2012 later");
            comboBox2.Items.Add("v4.0 | VisualStudio 2010 later");
            comboBox2.Items.Add("v3.5 | VisualStudio 2008 later");
            comboBox2.Items.Add("v3.0 | VisualStudio 2008 later");
            comboBox2.Items.Add("v2.0 | VisualStudio 2005 later");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            string flag = string.Empty;


            if (String.IsNullOrEmpty(textBox1.Text))
            {
                MessageBox.Show("sourceFolder is null or empty");
                flag = "X";
            }
            else if (String.IsNullOrEmpty(textBox2.Text))
            {
                if (MessageBox.Show("타겟폴더가 없으면 원본폴더에 내용이 수정됩니다. \r\n 원본폴더를 수정하시겠습니까?", "", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    flag = "Y";
                    sourceFolder = Path.GetFullPath(textBox1.Text.Trim());
                    targetFolder = Path.GetFullPath(textBox1.Text.Trim());
                }
                else
                {
                    flag = "N";
                }
            }
            else if (flag.Equals("Y"))
            {
                sourceFolder = Path.GetFullPath(textBox1.Text.Trim());
                targetFolder = Path.GetFullPath(textBox1.Text.Trim());
            }
            else if (flag.Equals("N"))
            {
                sourceFolder = Path.GetFullPath(textBox1.Text.Trim());
                targetFolder = Path.GetFullPath(textBox2.Text.Trim());
            }
            else
            {
                sourceFolder = Path.GetFullPath(textBox1.Text.Trim());
                targetFolder = Path.GetFullPath(textBox2.Text.Trim());
            }


            if (!flag.Equals("N") && !flag.Equals("X"))
            {
                //csproj 파일 찾기
                foreach (var item in GetSearchCsprojFile(sourceFolder))
                {
                    //타겟 파일명
                    //string _titem = Path.Combine(targetFolder, Path.GetFileName(item));
                    string _titem = Path.GetFullPath(item).Replace(sourceFolder, targetFolder);

                    //파일 변경
                    string returnItem = ChangeCsprojFile(Path.GetFullPath(item), _titem, toolVersion, netVersion);
                    richTextBox1.AppendText(returnItem + Environment.NewLine);
                }
            }


        }


        public string[] GetSearchCsprojFile(String _strPath) 
        { 
            string[] files = { "", }; 
            try { 
                files = Directory.GetFiles(_strPath, "*.csproj", SearchOption.AllDirectories); 
            } 
            catch (IOException ex) 
            { 
                MessageBox.Show(ex.Message); 
            } 
            return files; 
        }

        public string ChangeCsprojFile(String _sfile, String _tfile, String _toolver, String _netver)
        {
            if (String.IsNullOrEmpty(_toolver) && String.IsNullOrEmpty(_netver))
                return "변경된 파일이 없습니다.";

            // 소스폴더와 타겟폴더가 다르다면 타겟폴더에 파일 복사
            if (!_sfile.Equals(_tfile))
            {
                if (!Directory.Exists(Path.GetDirectoryName(_tfile)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(_tfile));
                }
                File.Copy(_sfile, _tfile);
            }

            //만약 읽기전용이면 해제한다.
            if (File.GetAttributes(_tfile) != FileAttributes.Normal)
            {
                File.SetAttributes(_tfile, FileAttributes.Normal);
            }

            string[] all_lines = File.ReadAllLines(_tfile);
            string fileName1 = string.Empty;
            string fileName2 = string.Empty;

            int index = 0;
            foreach (string line in all_lines)
            {
                if (!String.IsNullOrEmpty(_toolver) && line.Contains("ToolsVersion="))
                {
                    int _first = line.IndexOf("ToolsVersion=\"");
                    int first_location = _first + "ToolsVersion=".Length + 1;
                    int last_location = line.IndexOf("\">", first_location);
                    int replace_word_count = last_location - first_location;

                    string before_replace_word = line.Substring(first_location, replace_word_count);

                    all_lines[index] = line.Replace(before_replace_word, _toolver);
                    fileName1 = " => ToolsVersion=" + _toolver;
                }
                if (!String.IsNullOrEmpty(_netver) && line.Contains("<TargetFrameworkVersion>"))
                {
                    int _first = line.IndexOf("<TargetFrameworkVersion>");
                    int first_location = _first + "<TargetFrameworkVersion>".Length;
                    int last_location = line.IndexOf("</TargetFrameworkVersion>", first_location);
                    int replace_word_count = last_location - first_location;

                    string before_replace_word = line.Substring(first_location, replace_word_count);

                    all_lines[index] = line.Replace(before_replace_word, _netver);
                    fileName2 = " | TargetFrameworkVersion=" + _netver;
                }
                index++;
            }



            File.WriteAllLines(_tfile, all_lines);

            return _tfile + fileName1 + fileName2;
        }
            


    }
}
