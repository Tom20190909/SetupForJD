using SleckEc2KdInterface.Entity;
using SleckEc2KdInterface.DAL;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Windows.Forms;

namespace SleckEc2KdInterface
{
    public partial class Form1 : Form
    {
        DbContext dbContext = new DbContext();

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
                    
          
            var lst=dbContext.DeliverInfoDb.GetList(t => t.Platform_Code.Contains(textBox1.Text) || t.Code.Contains(textBox1.Text));      
            dataGridView6.DataSource = lst;
           
      //          var list = Db.Queryable<Student, School>((st, sc) => new object[] {
      //  JoinType.Left,st.SchoolId==sc.Id})
      //.Select((st, sc) => new { Id = st.Id, Name = st.Name, SchoolName = sc.Name }).ToList();
      //      var s11 = Db.Queryable<Student, School>((st, sc) => st.SchoolId == sc.Id)
      //                  .Select<ViewModelStudent3>().ToList();
        }

        private void dataGridView6_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
            {

              string fid=dataGridView6.Rows[e.RowIndex].Cells[0].Value.ToString();
              var itemlst = dbContext.DeliverItemDb.GetList(t => t.FID== fid);
              dataGridView1.DataSource = itemlst;

            }
        }
    }

}
