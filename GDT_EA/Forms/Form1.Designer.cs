namespace GDT_EA
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.dgvMCDC = new System.Windows.Forms.DataGridView();
            this.btn_gen = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnGetSituation = new System.Windows.Forms.Button();
            this.rtbFunc = new System.Windows.Forms.RichTextBox();
            this.cmbFunc = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbProject = new System.Windows.Forms.ComboBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnCreateTest = new System.Windows.Forms.Button();
            this.dgvTestData = new System.Windows.Forms.DataGridView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.rBtnOneSit = new System.Windows.Forms.RadioButton();
            this.rBtnAll = new System.Windows.Forms.RadioButton();
            this.rBtnGraph = new System.Windows.Forms.RadioButton();
            this.numTest = new System.Windows.Forms.NumericUpDown();
            this.pBar1 = new System.Windows.Forms.ProgressBar();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMCDC)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTestData)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTest)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvMCDC
            // 
            this.dgvMCDC.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMCDC.Location = new System.Drawing.Point(6, 19);
            this.dgvMCDC.Name = "dgvMCDC";
            this.dgvMCDC.Size = new System.Drawing.Size(411, 175);
            this.dgvMCDC.TabIndex = 0;
            // 
            // btn_gen
            // 
            this.btn_gen.Location = new System.Drawing.Point(27, 141);
            this.btn_gen.Name = "btn_gen";
            this.btn_gen.Size = new System.Drawing.Size(148, 33);
            this.btn_gen.TabIndex = 1;
            this.btn_gen.Text = "Сгенерировать данные";
            this.btn_gen.UseVisualStyleBackColor = true;
            this.btn_gen.Click += new System.EventHandler(this.btn_gen_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.btnGetSituation);
            this.groupBox1.Controls.Add(this.rtbFunc);
            this.groupBox1.Controls.Add(this.cmbFunc);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.cmbProject);
            this.groupBox1.Controls.Add(this.panel2);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(309, 484);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Настройки";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 116);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(78, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Тело функции";
            // 
            // btnGetSituation
            // 
            this.btnGetSituation.Location = new System.Drawing.Point(181, 83);
            this.btnGetSituation.Name = "btnGetSituation";
            this.btnGetSituation.Size = new System.Drawing.Size(122, 28);
            this.btnGetSituation.TabIndex = 7;
            this.btnGetSituation.Text = "Показать ситуации";
            this.btnGetSituation.UseVisualStyleBackColor = true;
            this.btnGetSituation.Click += new System.EventHandler(this.btnGetSituation_Click);
            // 
            // rtbFunc
            // 
            this.rtbFunc.Location = new System.Drawing.Point(7, 132);
            this.rtbFunc.Name = "rtbFunc";
            this.rtbFunc.Size = new System.Drawing.Size(296, 346);
            this.rtbFunc.TabIndex = 5;
            this.rtbFunc.Text = "";
            // 
            // cmbFunc
            // 
            this.cmbFunc.FormattingEnabled = true;
            this.cmbFunc.Location = new System.Drawing.Point(6, 88);
            this.cmbFunc.Name = "cmbFunc";
            this.cmbFunc.Size = new System.Drawing.Size(169, 21);
            this.cmbFunc.TabIndex = 4;
            this.cmbFunc.SelectedIndexChanged += new System.EventHandler(this.cmbFunc_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 71);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(86, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Выбор функции";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Выбор проекта";
            // 
            // cmbProject
            // 
            this.cmbProject.FormattingEnabled = true;
            this.cmbProject.Location = new System.Drawing.Point(6, 34);
            this.cmbProject.Name = "cmbProject";
            this.cmbProject.Size = new System.Drawing.Size(159, 21);
            this.cmbProject.TabIndex = 0;
            this.cmbProject.SelectedIndexChanged += new System.EventHandler(this.cmbProgect_SelectedIndexChanged);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.textBox1);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.button2);
            this.panel2.Location = new System.Drawing.Point(7, 15);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(280, 47);
            this.panel2.TabIndex = 6;
            this.panel2.Visible = false;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(7, 17);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(151, 20);
            this.textBox1.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 4);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(57, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Название";
            // 
            // button2
            // 
            this.button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button2.Location = new System.Drawing.Point(186, 14);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 29);
            this.button2.TabIndex = 2;
            this.button2.Text = "Добавить";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.dgvMCDC);
            this.groupBox2.Location = new System.Drawing.Point(327, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(445, 215);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Данные метода MC/DC";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.pBar1);
            this.groupBox3.Controls.Add(this.btnCreateTest);
            this.groupBox3.Controls.Add(this.dgvTestData);
            this.groupBox3.Controls.Add(this.panel1);
            this.groupBox3.Controls.Add(this.btn_gen);
            this.groupBox3.Location = new System.Drawing.Point(327, 235);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(445, 261);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Генерация данных";
            // 
            // btnCreateTest
            // 
            this.btnCreateTest.Location = new System.Drawing.Point(27, 211);
            this.btnCreateTest.Name = "btnCreateTest";
            this.btnCreateTest.Size = new System.Drawing.Size(148, 32);
            this.btnCreateTest.TabIndex = 3;
            this.btnCreateTest.Text = "Создать тест";
            this.btnCreateTest.UseVisualStyleBackColor = true;
            // 
            // dgvTestData
            // 
            this.dgvTestData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTestData.Location = new System.Drawing.Point(240, 19);
            this.dgvTestData.Name = "dgvTestData";
            this.dgvTestData.Size = new System.Drawing.Size(199, 236);
            this.dgvTestData.TabIndex = 2;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.rBtnOneSit);
            this.panel1.Controls.Add(this.rBtnAll);
            this.panel1.Controls.Add(this.rBtnGraph);
            this.panel1.Controls.Add(this.numTest);
            this.panel1.Location = new System.Drawing.Point(6, 19);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(228, 116);
            this.panel1.TabIndex = 0;
            // 
            // rBtnOneSit
            // 
            this.rBtnOneSit.AutoSize = true;
            this.rBtnOneSit.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.rBtnOneSit.Location = new System.Drawing.Point(3, 7);
            this.rBtnOneSit.Name = "rBtnOneSit";
            this.rBtnOneSit.Size = new System.Drawing.Size(169, 19);
            this.rBtnOneSit.TabIndex = 6;
            this.rBtnOneSit.TabStop = true;
            this.rBtnOneSit.Text = "Для ситуации с номером";
            this.rBtnOneSit.UseVisualStyleBackColor = true;
            // 
            // rBtnAll
            // 
            this.rBtnAll.AutoSize = true;
            this.rBtnAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.rBtnAll.Location = new System.Drawing.Point(3, 54);
            this.rBtnAll.Name = "rBtnAll";
            this.rBtnAll.Size = new System.Drawing.Size(133, 19);
            this.rBtnAll.TabIndex = 5;
            this.rBtnAll.TabStop = true;
            this.rBtnAll.Text = "Для всех ситуаций";
            this.rBtnAll.UseVisualStyleBackColor = true;
            // 
            // rBtnGraph
            // 
            this.rBtnGraph.AutoSize = true;
            this.rBtnGraph.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.rBtnGraph.Location = new System.Drawing.Point(3, 79);
            this.rBtnGraph.Name = "rBtnGraph";
            this.rBtnGraph.Size = new System.Drawing.Size(194, 19);
            this.rBtnGraph.TabIndex = 4;
            this.rBtnGraph.TabStop = true;
            this.rBtnGraph.Text = "По графу потока управления";
            this.rBtnGraph.UseVisualStyleBackColor = true;
            // 
            // numTest
            // 
            this.numTest.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.numTest.Location = new System.Drawing.Point(21, 32);
            this.numTest.Name = "numTest";
            this.numTest.Size = new System.Drawing.Size(63, 23);
            this.numTest.TabIndex = 2;
            // 
            // pBar1
            // 
            this.pBar1.Location = new System.Drawing.Point(27, 181);
            this.pBar1.Name = "pBar1";
            this.pBar1.Size = new System.Drawing.Size(157, 23);
            this.pBar1.TabIndex = 4;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 519);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.dgvMCDC)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTestData)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTest)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvMCDC;
        private System.Windows.Forms.Button btn_gen;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cmbProject;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RichTextBox rtbFunc;
        private System.Windows.Forms.ComboBox cmbFunc;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.DataGridView dgvTestData;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.NumericUpDown numTest;
        private System.Windows.Forms.Button btnCreateTest;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnGetSituation;
        private System.Windows.Forms.RadioButton rBtnOneSit;
        private System.Windows.Forms.RadioButton rBtnAll;
        private System.Windows.Forms.RadioButton rBtnGraph;
        private System.Windows.Forms.ProgressBar pBar1;
    }
}

