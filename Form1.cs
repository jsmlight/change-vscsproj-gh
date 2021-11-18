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
            toolVersion = _inputVal;
        }

        // .net version 변경
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            string _inputVal = comboBox2.SelectedItem.ToString().Trim();
            _inputVal = _inputVal.Split('|')[0].ToString();
            netVersion = _inputVal;
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

            if (String.IsNullOrEmpty(textBox1.Text))
            {
                MessageBox.Show("sourceFolder is null or empty");
            }
            else 
            {
                sourceFolder = Path.GetFullPath(textBox1.Text.Trim());
            }

            if (String.IsNullOrEmpty(textBox2.Text))
            {
                MessageBox.Show("원본폴더에 내용이 수정됨으로 타겟폴더를 지정해주세요.");
            }
            else 
            {
                targetFolder = Path.GetFullPath(textBox2.Text.Trim());
            }

            //csproj 파일 찾기
            foreach (var item in GetSearchCsprojFile(sourceFolder))
            {
                
                richTextBox1.AppendText(item + Environment.NewLine);

                //파일 변경
                ChangeCsprojFile(Path.GetFullPath(item), toolVersion, netVersion);
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

        public string ChangeCsprojFile(String _file, String _toolver, String _netver)
        {
            if (String.IsNullOrEmpty(_toolver) && String.IsNullOrEmpty(_netver))
                return "변경된 파일이 없습니다.";

            string[] all_lines = File.ReadAllLines(_file);

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
                }
                if (!String.IsNullOrEmpty(_netver) && line.Contains("<TargetFrameworkVersion>"))
                {
                    int _first = line.IndexOf("<TargetFrameworkVersion>");
                    int first_location = _first + "<TargetFrameworkVersion>".Length;
                    int last_location = line.IndexOf("</TargetFrameworkVersion>", first_location);
                    int replace_word_count = last_location - first_location;

                    string before_replace_word = line.Substring(first_location, replace_word_count);

                    all_lines[index] = line.Replace(before_replace_word, _netver);
                }
                index++;
            }

            File.WriteAllLines(_file, all_lines);

            return _file;
        }
            


    }
}
