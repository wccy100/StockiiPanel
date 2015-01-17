﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Drawing;
using System.IO;
using System.Data;
using System.Collections;//在C#中使用ArrayList必须引用Collections类

namespace StockiiPanel
{
    public partial class Form1 : Form
    {
        MyProgressBar myBar = new MyProgressBar();
        DataTable buffResult = new DataTable();//用于保留前一个数
        ArrayList headText = new ArrayList();

        private SerializableDictionary<string, string> rawDict = new SerializableDictionary<string, string>();
        private SerializableDictionary<string, string> sumDict = new SerializableDictionary<string, string>();
        private SerializableDictionary<string, string> customDict = new SerializableDictionary<string, string>();
        private SerializableDictionary<string, string> crossDict = new SerializableDictionary<string, string>();

        // 代理定义，可以在Invoke时传入相应的参数
        private delegate void funHandle(int nValue);
        private funHandle myHandle = null;

        /// <summary>
        /// 线程函数中调用的函数
        /// </summary>
        private void ShowProgressBar()
        {
            if (myBar == null || myBar.IsDisposed)
                myBar = new MyProgressBar();

            myHandle = new funHandle(myBar.SetProgressValue);
            //  myBar.MdiParent = this;
            myBar.StartPosition = FormStartPosition.CenterScreen;
            myBar.ShowDialog();
        }

        /// <summary>
        /// 线程函数，用于处理调用
        /// </summary>
        private void ThreadFun()
        {
            MethodInvoker mi = new MethodInvoker(ShowProgressBar);
            this.BeginInvoke(mi);

            System.Threading.Thread.Sleep(500); // sleep to show window

            int i = 0;
            while (!stop)
            {
                ++i;
                System.Threading.Thread.Sleep(50);
                // 这里直接调用代理
                this.Invoke(this.myHandle, new object[] { (i % 101) });
                if (i > 100)
                    i = 0;
            }
        }

        private bool stop = false;

        private void InitialCombo()
        {
            indexCombo.Items.Add("均价");
            indexCombo.Items.Add("振幅");
            indexCombo.Items.Add("换手");
            indexCombo.Items.Add("量比");
            indexCombo.Items.Add("总金额");
            indexCombo.Items.Add("涨幅");

            indexCombo.SelectedIndex = 0;

            for (int i = 3; i <= 30; ++i)
            {
                intervalCombo.Items.Add(i + "");
            }

            intervalCombo.SelectedIndex = 0;

            typeCombo.Items.Add("负和");
            typeCombo.Items.Add("所有和");
            typeCombo.Items.Add("正和");

            typeCombo.SelectedIndex = 0;

            compareCombo.Items.Add("指定两天减");
            compareCombo.Items.Add("指定时间段内的和");
            compareCombo.Items.Add("指定时间段内最大值比最小值");
            compareCombo.Items.Add("指定两天加");
            compareCombo.Items.Add("指定时间段内最大值减最小值");
            compareCombo.Items.Add("两个时间段时涨幅依据分段");
            compareCombo.Items.Add("指定两天比值");

            compareCombo.SelectedIndex = 0;

            compareIndexCombo.Items.Add("振幅");
            compareIndexCombo.Items.Add("流通股本");
            compareIndexCombo.Items.Add("均价流通值");
            compareIndexCombo.Items.Add("总股本");
            compareIndexCombo.Items.Add("均价");
            compareIndexCombo.Items.Add("现价");
            compareIndexCombo.Items.Add("总市值");
            compareIndexCombo.Items.Add("换手");
            compareIndexCombo.Items.Add("量比");
            compareIndexCombo.Items.Add("总金额");
            compareIndexCombo.Items.Add("涨幅");

            compareIndexCombo.SelectedIndex = 0;

            indexCombox1.Items.Add("均价");
            indexCombox1.Items.Add("总市值");
            indexCombox1.Items.Add("昨收");
            indexCombox1.Items.Add("流通股本");
            indexCombox1.Items.Add("均价流通市值");
            indexCombox1.Items.Add("总股本");

            indexCombox1.SelectedIndex = 0;
        }


        /// <summary>
        /// 判断一个数是否是奇数
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public bool IsOdd(int n)
        {
            if (n % 2 != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void rawContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            switch (curTabIndex)
            {
                case 0:
                    if (rawDataGrid.RowCount > 0)
                    {
                        for (int i = 0; i < rawContextMenuStrip.Items.Count; ++i)
                            rawContextMenuStrip.Items[i].Visible = true;
                    }
                    else
                    {
                        for (int i = 0; i < rawContextMenuStrip.Items.Count; ++i)
                            rawContextMenuStrip.Items[i].Visible = false;
                    }
                    combinePageToolStripMenuItem.Visible = false;
                    combineSelectToolStripMenuItem.Visible = false;
                    break;
                case 1:
                    if (ndayGrid.RowCount > 0)
                    {
                        for (int i = 0; i < rawContextMenuStrip.Items.Count; ++i)
                            rawContextMenuStrip.Items[i].Visible = true;
                    }
                    else
                    {
                        for (int i = 0; i < rawContextMenuStrip.Items.Count; ++i)
                            rawContextMenuStrip.Items[i].Visible = false;
                    }
                    combinePageToolStripMenuItem.Visible = false;
                    combineSelectToolStripMenuItem.Visible = false;
                    break;
                case 2:
                    if (calResultGrid.RowCount > 0)
                    {
                        for (int i = 0; i < rawContextMenuStrip.Items.Count; ++i)
                            rawContextMenuStrip.Items[i].Visible = true;
                    }
                    else
                    {
                        for (int i = 0; i < rawContextMenuStrip.Items.Count; ++i)
                            rawContextMenuStrip.Items[i].Visible = false;
                    }
                    break;
                case 3:
                    if (sectionResultGrid.RowCount > 0)
                    {
                        for (int i = 0; i < rawContextMenuStrip.Items.Count; ++i)
                            rawContextMenuStrip.Items[i].Visible = true;
                    }
                    else
                    {
                        for (int i = 0; i < rawContextMenuStrip.Items.Count; ++i)
                            rawContextMenuStrip.Items[i].Visible = false;
                    }
                    break;
                default:
                    break;
            }
        }


        private void startDatePicker_ValueChanged(object sender, EventArgs e)
        {
            startDatePicker1.Value = startDatePicker.Value;
            startDatePicker2.Value = startDatePicker.Value;
            startDatePicker3.Value = startDatePicker.Value;
        }

        private void endDatePicker_ValueChanged(object sender, EventArgs e)
        {
            endDatePicker1.Value = endDatePicker.Value;
            endDatePicker2.Value = endDatePicker.Value;
            endDatePicker3.Value = endDatePicker.Value;
        }

        private void startDatePicker1_ValueChanged(object sender, EventArgs e)
        {
            startDatePicker.Value = startDatePicker1.Value;
            startDatePicker2.Value = startDatePicker1.Value;
            startDatePicker3.Value = startDatePicker1.Value;
        }

        private void endDatePicker1_ValueChanged(object sender, EventArgs e)
        {
            endDatePicker.Value = endDatePicker1.Value;
            endDatePicker2.Value = endDatePicker1.Value;
            endDatePicker3.Value = endDatePicker1.Value;
        }

        private void startDatePicker2_ValueChanged(object sender, EventArgs e)
        {
            startDatePicker1.Value = startDatePicker2.Value;
            startDatePicker.Value = startDatePicker2.Value;
            startDatePicker3.Value = startDatePicker2.Value;
        }

        private void startDatePicker3_ValueChanged(object sender, EventArgs e)
        {
            startDatePicker1.Value = startDatePicker3.Value;
            startDatePicker2.Value = startDatePicker3.Value;
            startDatePicker.Value = startDatePicker3.Value;
        }

        private void endDatePicker3_ValueChanged(object sender, EventArgs e)
        {
            endDatePicker1.Value = endDatePicker3.Value;
            endDatePicker2.Value = endDatePicker3.Value;
            endDatePicker.Value = endDatePicker3.Value;
        }


        private void endDatePicker2_ValueChanged(object sender, EventArgs e)
        {
            endDatePicker1.Value = endDatePicker2.Value;
            endDatePicker.Value = endDatePicker2.Value;
            endDatePicker3.Value = endDatePicker2.Value;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //序列化保存
            using (FileStream fileStream = new FileStream("group.xml", FileMode.Create))
            {
                XmlSerializer xmlFormatter = new XmlSerializer(typeof(SerializableDictionary<string, ArrayList>));
                xmlFormatter.Serialize(fileStream, this.pList);
            }
        }

        private void saveTableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            switch (curTabIndex)
            {
                case 0:
                    dt = Commons.StructrueDataTable(rawDataGrid, false);
                    break;
                case 1:
                    dt = Commons.StructrueDataTable(ndayGrid, false);
                    break;
                case 2:
                    dt = Commons.StructrueDataTable(calResultGrid, false);
                    break;
                case 3:
                    dt = Commons.StructrueDataTable(sectionResultGrid, false);
                    break;
                default:
                    break;
            }

            Commons.ExportDataGridToCSV(dt);
        }

        private void saveSelectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            switch (curTabIndex)
            {
                case 0:
                    dt = Commons.StructrueDataTable(rawDataGrid, true);
                    break;
                case 1:
                    dt = Commons.StructrueDataTable(ndayGrid, true);
                    break;
                case 2:
                    dt = Commons.StructrueDataTable(calResultGrid, true);
                    break;
                case 3:
                    dt = Commons.StructrueDataTable(sectionResultGrid, true);
                    break;
                default:
                    break;
            }

            Commons.ExportDataGridToCSV(dt);
        }


        private void sumWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            myBar.Close();
            if (stop)//因异常停止
            {
                switch (errorNo)
                {
                    case 0:
                        MessageBox.Show("超时，不能连接到数据库");
                        break;
                    case 24:
                        MessageBox.Show("内存溢出");
                        break;
                    case 1045:
                        MessageBox.Show("无效的用户名密码");
                        break;
                    case 1049:
                        MessageBox.Show("数据库不存在");
                        break;
                    default:
                        MessageBox.Show("连接数据库错误");
                        break;
                }
                return;
            }

            dt = (DataTable)ds.Tables["n_day_sum"];

            if (dt == null || dt.Rows.Count == 0)
            {
                stop = true;
                MessageBox.Show("无查询结果");
                return;
            }
            DataSet orgDs = (DataSet)ndayGrid.DataSource;
            if (orgDs != null && orgDs.Tables["n_day_sum"] != null && pageList[curTabIndex] != 1)
            {
                orgDs.Tables["n_day_sum"].Merge(dt);
            }
            else
            {
                ndayGrid.DataSource = ds;
                ndayGrid.DataMember = "n_day_sum";
            }

            pageLabel1.Text = pageList[curTabIndex] + "/" + (int)totalPageList[curTabIndex];

            ndayGrid.AllowUserToAddRows = false;//不显示最后空白行
            ndayGrid.EnableHeadersVisualStyles = false;

            /*
            //改变DataGridView的表头
            ndayGrid.Columns[0].HeaderText = "代码";
            ndayGrid.Columns[0].Width = 70;
            ndayGrid.Columns[0].DataPropertyName = ds.Tables[0].Columns["stock_id"].ToString();
            ndayGrid.Columns[0].Frozen = true;

            ndayGrid.Columns[1].HeaderText = "名称";
            ndayGrid.Columns[1].Width = 80;
            ndayGrid.Columns[1].DataPropertyName = ds.Tables[0].Columns["stock_name"].ToString();
            ndayGrid.Columns[1].Frozen = true;

            ndayGrid.Columns[2].HeaderText = "开始日期";
            ndayGrid.Columns[2].Width = 110;
            ndayGrid.Columns[2].DataPropertyName = ds.Tables[0].Columns["start_date"].ToString();

            ndayGrid.Columns[3].HeaderText = "结束日期";
            ndayGrid.Columns[3].Width = 110;
            ndayGrid.Columns[3].DataPropertyName = ds.Tables[0].Columns["end_date"].ToString();

            if (daySumButton.Checked)
            {
                ndayGrid.Columns[4].HeaderText = intervalCombo.Text + "日和（元）";
                ndayGrid.Columns[4].Width = 110;
                ndayGrid.Columns[4].DataPropertyName = ds.Tables[0].Columns["value"].ToString();
            }
            else if (weekSumButton.Checked)
            {
                ndayGrid.Columns[4].HeaderText = intervalCombo.Text + "周和（元）";
                ndayGrid.Columns[4].Width = 110;
                ndayGrid.Columns[4].DataPropertyName = ds.Tables[0].Columns["value"].ToString();
            }
            else if (monthSumButton.Checked)
            {
                ndayGrid.Columns[4].HeaderText = intervalCombo.Text + "月和（元）";
                ndayGrid.Columns[4].Width = 110;
                ndayGrid.Columns[4].DataPropertyName = ds.Tables[0].Columns["value"].ToString();
            }
             */
            if (daySumButton.Checked)
            {
                using (FileStream fileStream = new FileStream("sumDay.xml", FileMode.Open))
                {
                    XmlSerializer xmlFormatter = new XmlSerializer(typeof(SerializableDictionary<string, string>));
                    this.sumDict = (SerializableDictionary<string, string>)xmlFormatter.Deserialize(fileStream);
                }
            }
            else if (weekSumButton.Checked)
            {
                using (FileStream fileStream = new FileStream("sumWeek.xml", FileMode.Open))
                {
                    XmlSerializer xmlFormatter = new XmlSerializer(typeof(SerializableDictionary<string, string>));
                    this.sumDict = (SerializableDictionary<string, string>)xmlFormatter.Deserialize(fileStream);
                }
            }
            else if (monthSumButton.Checked)
            {
                using (FileStream fileStream = new FileStream("sumMonth.xml", FileMode.Open))
                {
                    XmlSerializer xmlFormatter = new XmlSerializer(typeof(SerializableDictionary<string, string>));
                    this.sumDict = (SerializableDictionary<string, string>)xmlFormatter.Deserialize(fileStream);
                }
            }
            int k = 0;
            int sortIndex = 0;
            foreach (var item in sumDict)
            {
                ndayGrid.Columns[k].HeaderText = item.Value;
                ndayGrid.Columns[k].Width = 60 + item.Value.Length * 10;
                ndayGrid.Columns[k].DataPropertyName = ds.Tables[0].Columns[k].ToString();
                ndayGrid.Columns[k].SortMode = DataGridViewColumnSortMode.Programmatic;
                switch (item.Key)
                {
                    case "stock_id":
                    case "stock_name":
                        ndayGrid.Columns[k].Frozen = true;
                        break;
                    case "value":
                        ndayGrid.Columns[k].HeaderText = intervalCombo.Text + item.Value;
                        ndayGrid.Columns[k].Width = 60 + ndayGrid.Columns[k].HeaderText.Length * 10;
                        break;
                    default:
                        break;
                }
                if (sortColumnList[curTabIndex] == ndayGrid.Columns[k].DataPropertyName)
                {
                    sortIndex = k;
                }
                k++;
            }
            ndayGrid.Columns[sortIndex].HeaderCell.SortGlyphDirection = sortTypeList[curTabIndex] ? SortOrder.Ascending : SortOrder.Descending;
            ndayGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            ndayGrid.AutoResizeColumns();
            Commons.sumNum = sumDict.Count;

            stop = true;
        }

        private void customWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            myBar.Close();
            if (stop)//因异常停止
            {
                switch (errorNo)
                {
                    case 0:
                        MessageBox.Show("超时，不能连接到数据库");
                        break;
                    case 24:
                        MessageBox.Show("内存溢出");
                        break;
                    case 1045:
                        MessageBox.Show("无效的用户名密码");
                        break;
                    case 1049:
                        MessageBox.Show("数据库不存在");
                        break;
                    default:
                        MessageBox.Show("连接数据库错误");
                        break;
                }
                return;
            }
            String tableName = "";
            foreach (DataTable d in ds.Tables)
            {
                switch (d.TableName)
                {
                    case "stock_day_diff":
                        dt = d;
                        tableName = "stock_day_diff";
                        break;
                    case "stock_day_diff_seperate":
                        dt = d;
                        tableName = "stock_day_diff_seperate";
                        break;
                    case "stock_day_diff_sum":
                        dt = d;
                        tableName = "stock_day_diff_sum";
                        break;
                    default:
                        dt = null;
                        break;

                }
                
            }
            if (dt == null || dt.Rows.Count == 0)
            {
                stop = true;
                MessageBox.Show("无查询结果");
                return;
            }

            pageLabel.Text = pageList[curTabIndex] + "/" + (int)totalPageList[curTabIndex];

            calResultGrid.DataSource = ds;
            calResultGrid.DataMember = tableName;

            calResultGrid.AllowUserToAddRows = false;//不显示最后空白行
            calResultGrid.EnableHeadersVisualStyles = false;
            /*
            //改变DataGridView的表头
            calResultGrid.Columns[0].HeaderText = "代码";
            calResultGrid.Columns[0].Width = 70;
            calResultGrid.Columns[0].DataPropertyName = ds.Tables[0].Columns["stock_id"].ToString();
            calResultGrid.Columns[0].Frozen = true;

            calResultGrid.Columns[1].HeaderText = "名称";
            calResultGrid.Columns[1].Width = 80;
            calResultGrid.Columns[1].DataPropertyName = ds.Tables[0].Columns["stock_name"].ToString();
            calResultGrid.Columns[1].Frozen = true;

            calResultGrid.Columns[2].HeaderText = "开始时间";
            calResultGrid.Columns[2].Width = 110;
            calResultGrid.Columns[2].DataPropertyName = ds.Tables[0].Columns["start_date"].ToString();

            calResultGrid.Columns[3].HeaderText = "结束时间";
            calResultGrid.Columns[3].Width = 110;
            calResultGrid.Columns[3].DataPropertyName = ds.Tables[0].Columns["end_date"].ToString();

            switch (tableName)
            {
                case "stock_day_diff":
                    calResultGrid.Columns[4].HeaderText = "起始时间的值";
                    calResultGrid.Columns[4].Width = 150;
                    calResultGrid.Columns[4].DataPropertyName = ds.Tables[0].Columns["start_value"].ToString();

                    calResultGrid.Columns[5].HeaderText = "结束时间的值";
                    calResultGrid.Columns[5].Width = 150;
                    calResultGrid.Columns[5].DataPropertyName = ds.Tables[0].Columns["end_value"].ToString();

                    calResultGrid.Columns[6].HeaderText = compareIndexCombo.Text + "(" + compareCombo.Text + ")";
                    calResultGrid.Columns[6].Width = 240;
                    calResultGrid.Columns[6].DataPropertyName = ds.Tables[0].Columns["index_value"].ToString();
                    break;
                case "stock_day_diff_seperate":
                    calResultGrid.Columns[4].HeaderText = "涨幅总数";
                    calResultGrid.Columns[4].Width = 110;
                    calResultGrid.Columns[4].DataPropertyName = ds.Tables[0].Columns["growth_count"].ToString();

                    calResultGrid.Columns[5].HeaderText = "振幅总数";
                    calResultGrid.Columns[5].Width = 110;
                    calResultGrid.Columns[5].DataPropertyName = ds.Tables[0].Columns["amp_count"].ToString();

                    calResultGrid.Columns[6].HeaderText = compareIndexCombo.Text + "(" + compareCombo.Text + ")";
                    calResultGrid.Columns[6].Width = 240;
                    calResultGrid.Columns[6].DataPropertyName = ds.Tables[0].Columns["index_value"].ToString();
                    break;
                case "stock_day_diff_sum":
                    calResultGrid.Columns[4].HeaderText = "起始时间的值";
                    calResultGrid.Columns[4].Width = 150;
                    calResultGrid.Columns[4].DataPropertyName = ds.Tables[0].Columns["start_value"].ToString();

                    calResultGrid.Columns[5].HeaderText = "结束时间的值";
                    calResultGrid.Columns[5].Width = 150;
                    calResultGrid.Columns[5].DataPropertyName = ds.Tables[0].Columns["end_value"].ToString();

                    calResultGrid.Columns[6].HeaderText = compareIndexCombo.Text + "(" + compareCombo.Text + ")";
                    calResultGrid.Columns[6].Width = 240;
                    calResultGrid.Columns[6].DataPropertyName = ds.Tables[0].Columns["index_value"].ToString();
                    break;
                default:
                    break;

            }
            */
            switch (tableName)
            {
                case "stock_day_diff":
                    switch (compareCombo.Text)
                    {
                        case "指定时间段内最大值减最小值":
                        case "指定时间段内最大值比最小值":
                            using (FileStream fileStream = new FileStream("customMaxMin.xml", FileMode.Open))
                            {
                                XmlSerializer xmlFormatter = new XmlSerializer(typeof(SerializableDictionary<string, string>));
                                this.customDict = (SerializableDictionary<string, string>)xmlFormatter.Deserialize(fileStream);
                            }
                            break;
                        default:
                            using (FileStream fileStream = new FileStream("custom.xml", FileMode.Open))
                            {
                                XmlSerializer xmlFormatter = new XmlSerializer(typeof(SerializableDictionary<string, string>));
                                this.customDict = (SerializableDictionary<string, string>)xmlFormatter.Deserialize(fileStream);
                            }
                            break;
                    }
                    
                    break;
                case "stock_day_diff_seperate":
                    using (FileStream fileStream = new FileStream("customSeperate.xml", FileMode.Open))
                    {
                        XmlSerializer xmlFormatter = new XmlSerializer(typeof(SerializableDictionary<string, string>));
                        this.customDict = (SerializableDictionary<string, string>)xmlFormatter.Deserialize(fileStream);
                    }
                    break;
                case "stock_day_diff_sum":
                    using (FileStream fileStream = new FileStream("customSum.xml", FileMode.Open))
                    {
                        XmlSerializer xmlFormatter = new XmlSerializer(typeof(SerializableDictionary<string, string>));
                        this.customDict = (SerializableDictionary<string, string>)xmlFormatter.Deserialize(fileStream);
                    }
                    break;
                default:
                    break;
            }
            int k = 0;
            foreach (var item in customDict)
            {
                calResultGrid.Columns[k].HeaderText = item.Value;
                calResultGrid.Columns[k].Width = 60 + item.Value.Length * 10;
                calResultGrid.Columns[k].DataPropertyName = ds.Tables[0].Columns[item.Key].ToString();

                switch (item.Key)
                {
                    case "stock_id":
                    case "stock_name":
                        calResultGrid.Columns[k].Frozen = true;
                        break;
                    case "index_value":
                        calResultGrid.Columns[k].HeaderText = compareIndexCombo.Text + "(" + compareCombo.Text + ")";
                        calResultGrid.Columns[k].Width = 60 + calResultGrid.Columns[k].HeaderText.Length * 10;
                        break;
                    default:
                        break;
                }
                k++;
            }
            Commons.customNum = customDict.Count;

            //除了涨幅、振幅分段的计算是冻结6列之外，别的计算都是冻结2列
            if (compareCombo.Text == "两个时间段时涨幅依据分段")
            {
                for (int i = 2; i < 6; i++)
                {
                    calResultGrid.Columns[i].Frozen = true;
                }
            }

            stop = true;
        }

        private void crossWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            myBar.Close();
            if (stop)//因异常停止
            {
                switch (errorNo)
                {
                    case 0:
                        MessageBox.Show("超时，不能连接到数据库");
                        break;
                    case 24:
                        MessageBox.Show("内存溢出");
                        break;
                    case 1045:
                        MessageBox.Show("无效的用户名密码");
                        break;
                    case 1049:
                        MessageBox.Show("数据库不存在");
                        break;
                    default:
                        MessageBox.Show("连接数据库错误");
                        break;
                }
                return;
            }

            dt = (DataTable)ds.Tables["cross_info"];

            if (dt == null || dt.Rows.Count == 0)
            {
                stop = true;
                MessageBox.Show("无查询结果");
                return;
            }

            pageLabel.Text = pageList[curTabIndex] + "/" + (int)totalPageList[curTabIndex];

            sectionResultGrid.DataSource = ds;
            sectionResultGrid.DataMember = "cross_info";

            sectionResultGrid.AllowUserToAddRows = false;//不显示最后空白行
            sectionResultGrid.EnableHeadersVisualStyles = false;

            /*
            //改变DataGridView的表头
            sectionResultGrid.Columns[0].HeaderText = "代码";
            sectionResultGrid.Columns[0].Width = 70;
            sectionResultGrid.Columns[0].DataPropertyName = ds.Tables[0].Columns["stock_id"].ToString();
            sectionResultGrid.Columns[0].Frozen = true;

            sectionResultGrid.Columns[1].HeaderText = "名称";
            sectionResultGrid.Columns[1].Width = 80;
            sectionResultGrid.Columns[1].DataPropertyName = ds.Tables[0].Columns["stock_name"].ToString();
            sectionResultGrid.Columns[1].Frozen = true;

            sectionResultGrid.Columns[2].HeaderText = "开始日期";
            sectionResultGrid.Columns[2].Width = 110;
            sectionResultGrid.Columns[2].DataPropertyName = ds.Tables[0].Columns["start_date"].ToString();

            sectionResultGrid.Columns[3].HeaderText = "起始时间的值";
            sectionResultGrid.Columns[3].Width = 150;
            sectionResultGrid.Columns[3].DataPropertyName = ds.Tables[0].Columns["start_value"].ToString();

            sectionResultGrid.Columns[4].HeaderText = "结束日期";
            sectionResultGrid.Columns[4].Width = 110;
            sectionResultGrid.Columns[4].DataPropertyName = ds.Tables[0].Columns["end_date"].ToString();

            sectionResultGrid.Columns[5].HeaderText = "结束时间的值";
            sectionResultGrid.Columns[5].Width = 150;
            sectionResultGrid.Columns[5].DataPropertyName = ds.Tables[0].Columns["end_value"].ToString();

            sectionResultGrid.Columns[6].HeaderText = "开始－上市（天）";
            sectionResultGrid.Columns[6].Width = 150;
            sectionResultGrid.Columns[6].DataPropertyName = ds.Tables[0].Columns["start_list_date"].ToString();

            sectionResultGrid.Columns[7].HeaderText = "结束－上市（天）";
            sectionResultGrid.Columns[7].Width = 150;
            sectionResultGrid.Columns[7].DataPropertyName = ds.Tables[0].Columns["end_list_date"].ToString();

            sectionResultGrid.Columns[8].HeaderText = "跨区类型";
            sectionResultGrid.Columns[8].Width = 120;
            sectionResultGrid.Columns[8].DataPropertyName = ds.Tables[0].Columns["cross_type"].ToString();

            sectionResultGrid.Columns[9].HeaderText = "均值(" + indexCombox1.Text + ")";
            sectionResultGrid.Columns[9].Width = 180;
            sectionResultGrid.Columns[9].DataPropertyName = ds.Tables[0].Columns["avg"].ToString();

            sectionResultGrid.Columns[10].HeaderText = "差异(" + indexCombox1.Text + ")";
            sectionResultGrid.Columns[10].Width = 180;
            sectionResultGrid.Columns[10].DataPropertyName = ds.Tables[0].Columns["difference"].ToString();
            */
            int k = 0;
            foreach (var item in crossDict)
            {
                sectionResultGrid.Columns[k].HeaderText = item.Value;
                sectionResultGrid.Columns[k].Width = 60 + item.Value.Length * 10;
                sectionResultGrid.Columns[k].DataPropertyName = ds.Tables[0].Columns[item.Key].ToString();

                switch (item.Key)
                {
                    case "stock_id":
                    case "stock_name":
                        sectionResultGrid.Columns[k].Frozen = true;
                        break;
                    case "avg":
                    case "difference":
                        sectionResultGrid.Columns[k].HeaderText = item.Value + "(" + compareCombo.Text + ")";
                        sectionResultGrid.Columns[k].Width = 60 + sectionResultGrid.Columns[k].HeaderText.Length * 10;
                        break;
                    default:
                        break;
                }
                k++;
            }
            Commons.crossNum = crossDict.Count;

            stop = true;
        }


        private void ndayGrid_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {

            DataGridViewTextBoxColumn dgv_Text = new DataGridViewTextBoxColumn();
            //自动整理序列号
            int coun = ndayGrid.RowCount;
            for (int i = 0; i < coun; i++)
            {
                int j = i + 1;
                ndayGrid.Rows[i].HeaderCell.Value = j.ToString();

                //隔行显示不同的颜色
                if (IsOdd(i))
                {
                    ndayGrid.Rows[i].DefaultCellStyle.BackColor = Color.AliceBlue;
                }

                //前两行显示绿色
                ndayGrid.Rows[i].Cells[0].Style.BackColor = Color.Lime;
                ndayGrid.Rows[i].Cells[1].Style.BackColor = Color.Lime;
            }
        }

        private void calResultGrid_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            DataGridViewTextBoxColumn dgv_Text = new DataGridViewTextBoxColumn();
            //自动整理序列号
            int coun = calResultGrid.RowCount;
            for (int i = 0; i < coun; i++)
            {
                int j = i + 1;
                calResultGrid.Rows[i].HeaderCell.Value = j.ToString();

                //隔行显示不同的颜色
                if (IsOdd(i))
                {
                    calResultGrid.Rows[i].DefaultCellStyle.BackColor = Color.AliceBlue;
                }

                //前两行显示绿色
                calResultGrid.Rows[i].Cells[0].Style.BackColor = Color.Lime;
                calResultGrid.Rows[i].Cells[1].Style.BackColor = Color.Lime;
                if (compareCombo.Text == "两个时间段时涨幅依据分段")
                {
                    for (int k = 2; k < 6; k++)
                    {
                        calResultGrid.Rows[i].Cells[k].Style.BackColor = Color.Lime;
                    }
                }
            }
        }

        private void sectionResultGrid_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            DataGridViewTextBoxColumn dgv_Text = new DataGridViewTextBoxColumn();
            //自动整理序列号
            int coun = sectionResultGrid.RowCount;
            for (int i = 0; i < coun; i++)
            {
                int j = i + 1;
                sectionResultGrid.Rows[i].HeaderCell.Value = j.ToString();

                if (ds.Tables[0].Rows[i]["cross_type"].ToString() == "positive")
                {
                    sectionResultGrid.Rows[i].DefaultCellStyle.BackColor = Color.Red;
                }
                else if (ds.Tables[0].Rows[i]["cross_type"].ToString() == "negative")
                {
                    sectionResultGrid.Rows[i].DefaultCellStyle.BackColor = Color.Lime;
                }
                else
                {
                    sectionResultGrid.Rows[i].DefaultCellStyle.BackColor = Color.Yellow;
                }

            }
        }

        private void combineResult_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            //自动整理序列号
            int coun = combineResult.RowCount;
            for (int i = 0; i < coun; i++)
            {
                int j = i + 1;
                combineResult.Rows[i].HeaderCell.Value = j.ToString();
            }
        }

        /// <summary>
        /// 版块中设置时间
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="num">参数</param>
        /// <param name="year">年份</param>
        private void SetDate(String type, String num, String year)
        {
            int arg = Convert.ToInt32(num);
            int years = Convert.ToInt32(year);
            int month = 1;

            DateTime startDate = new DateTime();
            DateTime endDate = new DateTime();

            if (type.Equals("season"))//季度
            {
                switch (arg)
                {
                    case 1:
                        month = 1;
                        startDate = new DateTime(years, month, 1);
                        month = 3;
                        endDate = new DateTime(years, month, 1);
                        break;
                    case 2:
                        month = 4;
                        startDate = new DateTime(years, month, 1);
                        month = 6;
                        endDate = new DateTime(years, month, 1);
                        break;
                    case 3:
                        month = 7;
                        startDate = new DateTime(years, month, 1);
                        month = 9;
                        endDate = new DateTime(years, month, 1);
                        break;
                    case 4:
                        month = 10;
                        startDate = new DateTime(years, month, 1);
                        month = 12;
                        endDate = new DateTime(years, month, 1);
                        break;
                    default:
                        break;
                }
            }
            else //月份
            {
                month = arg;
                startDate = new DateTime(years, month, 1);
                endDate = new DateTime(years, month, 1);
            }

            startDatePicker.Value = TimeControl.GetFirstDayOfMonth(startDate);
            startDatePicker1.Value = TimeControl.GetFirstDayOfMonth(startDate);
            startDatePicker2.Value = TimeControl.GetFirstDayOfMonth(startDate);
            startDatePicker3.Value = TimeControl.GetFirstDayOfMonth(startDate);

            endDatePicker.Value = TimeControl.GetLastDayOfMonth(endDate);
            endDatePicker1.Value = TimeControl.GetLastDayOfMonth(endDate);
            endDatePicker2.Value = TimeControl.GetLastDayOfMonth(endDate);
            endDatePicker3.Value = TimeControl.GetLastDayOfMonth(endDate);
        }

        private void searchTab(int type)
        {
            if (!Commons.isTradeDay(startDatePicker.Value) || !Commons.isTradeDay(endDatePicker.Value))
            {
                MessageBox.Show("非交易日");
                return;
            }

            if (type < 3 && groupList.Visible == true && groupList.SelectedItems.Count == 0)
            {
                MessageBox.Show("请选择一个分组");
                return;
            }
            else if (type < 3 && groupList.Visible == false && record.Count == 0)
            {
                MessageBox.Show("请选择一个版块");
                return;
            }

            // 启动Loading线程
            System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(ThreadFun));
            thread.Start();

            //启动后台线程
            stop = false;

            String args;
            if (groupList.Visible && type < 3)//自选
            {
                args = startDatePicker.Value.ToString("yyyy-MM-dd") + "," + endDatePicker.Value.ToString("yyyy-MM-dd") + "," + groupList.SelectedItem.ToString();
            }
            else
            {
                args = startDatePicker.Value.ToString("yyyy-MM-dd") + "," + endDatePicker.Value.ToString("yyyy-MM-dd") ;
            }

            switch (type)
            {
                case 0:
                    bkWorker.RunWorkerAsync(args);
                    //pageLabel.Text = "0/0";
                    break;
                case 1:
                    args += "," + intervalCombo.Text + "," + indexCombo.Text + "," + typeCombo.Text;
                    sumWorker.RunWorkerAsync(args);
                    //pageLabel1.Text = "0/0";
                    break;
                case 2:
                    string min = smallBox.Text;
                    string max = bigBox.Text;
                    if (smallBox.Text.Equals(""))
                        min = "0";
                    if (bigBox.Text.Equals(""))
                        max = "0";
                    args += "," + min + "," + max + "," + compareIndexCombo.Text + "," + compareCombo.Text;
                    customWorker.RunWorkerAsync(args);
                    break;
                case 3:
                    if (weighBox.Text.Equals(""))
                    {
                        MessageBox.Show("请输入权重");
                        stop = true;
                        myBar.Close();
                        return;
                    }
                    crossWorker.RunWorkerAsync(startDatePicker.Value.ToString("yyyy-MM-dd") + "," + endDatePicker.Value.ToString("yyyy-MM-dd") + "," + weighBox.Text + "," + indexCombox1.Text);
                    break;
                default:
                    break;
            }

        }


        private void combinePageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Combine(false);
        }

        private void combineSelectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Combine(true);
        }

        private void Combine(bool isSelect)
        {
            buffResult = (DataTable)combineResult.DataSource;//保存上一个
            headText.Clear();
            for (int i = 0; i < combineResult.ColumnCount; i++)
                headText.Add(combineResult.Columns[i].HeaderText);
           
            if (curTabIndex == 2)
            {
                combineResult.DataSource = Commons.Combine(calResultGrid, combineResult, isSelect);
                if (combineResult.ColumnCount == calResultGrid.ColumnCount)//第一次拼接
                {
                    for (int i = 0; i < combineResult.ColumnCount; i++)
                        combineResult.Columns[i].HeaderText = calResultGrid.Columns[i].HeaderText;
                }
                else
                {
                    int length = buffResult.Columns.Count;

                    for (int i = 0; i < calResultGrid.ColumnCount; i++)
                        combineResult.Columns[length + i].HeaderText = calResultGrid.Columns[i].HeaderText;
                }
            }
            else
            {
                combineResult.DataSource = Commons.Combine(sectionResultGrid, combineResult, isSelect);
                if (combineResult.ColumnCount == sectionResultGrid.ColumnCount)//第一次拼接
                {
                    for (int i = 0; i < combineResult.ColumnCount; i++)
                        combineResult.Columns[i].HeaderText = sectionResultGrid.Columns[i].HeaderText;
                }
                else
                {
                    int length = buffResult.Columns.Count;

                    for (int i = 0; i < sectionResultGrid.ColumnCount; i++)
                        combineResult.Columns[length + i].HeaderText = sectionResultGrid.Columns[i].HeaderText;
                }
            }

        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DataTable tmp = buffResult;
            ArrayList tmpArray = (ArrayList)headText.Clone();
            
            buffResult = (DataTable)combineResult.DataSource;//保存上一个
            headText.Clear();
            for (int i = 0; i < combineResult.ColumnCount; i++)
                headText.Add(combineResult.Columns[i].HeaderText);

            combineResult.DataSource = tmp;
            for (int i = 0; i < tmpArray.Count; i++)
                combineResult.Columns[i].HeaderText = tmpArray[i].ToString();
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            buffResult = (DataTable)combineResult.DataSource;//保存上一个
            headText.Clear();
            for (int i = 0; i < combineResult.ColumnCount; i++)
                headText.Add(combineResult.Columns[i].HeaderText);
            combineResult.DataSource = null;
        }
    }


}
