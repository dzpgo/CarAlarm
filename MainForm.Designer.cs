namespace CarAlarm
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Main_Panel = new Panel();
            Mian_TableLayoutPanel = new TableLayoutPanel();
            tableLayoutPanel1 = new TableLayoutPanel();
            groupBox1 = new GroupBox();
            lbl_COMAvailable = new Label();
            label7 = new Label();
            lbl_FinalTime = new Label();
            lbl_CarNo = new Label();
            lbl_CarName = new Label();
            lbl_COMOccupy = new Label();
            lbl_COMStatus = new Label();
            lbl_ServerStatus = new Label();
            label6 = new Label();
            label5 = new Label();
            label4 = new Label();
            label3 = new Label();
            label2 = new Label();
            label1 = new Label();
            groupBox2 = new GroupBox();
            cb_Sound = new CheckBox();
            cb_Light = new CheckBox();
            label8 = new Label();
            cmb_ComPorts = new ComboBox();
            btn_CloseLight = new Button();
            btn_TestLight = new Button();
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            dgv_AllEntryRecords = new DataGridView();
            tabPage2 = new TabPage();
            dgv_BlacklistEntryRecords = new DataGridView();
            tabPage3 = new TabPage();
            dgv_Blacklist = new DataGridView();
            tbpg_Settings = new TabPage();
            bt_ComOpen = new Button();
            tb_Port = new TextBox();
            tb_ServerURL = new TextBox();
            label10 = new Label();
            label9 = new Label();
            Main_Panel.SuspendLayout();
            Mian_TableLayoutPanel.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgv_AllEntryRecords).BeginInit();
            tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgv_BlacklistEntryRecords).BeginInit();
            tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgv_Blacklist).BeginInit();
            tbpg_Settings.SuspendLayout();
            SuspendLayout();
            // 
            // Main_Panel
            // 
            Main_Panel.Controls.Add(Mian_TableLayoutPanel);
            Main_Panel.Dock = DockStyle.Fill;
            Main_Panel.Location = new Point(0, 0);
            Main_Panel.Name = "Main_Panel";
            Main_Panel.Size = new Size(395, 446);
            Main_Panel.TabIndex = 0;
            // 
            // Mian_TableLayoutPanel
            // 
            Mian_TableLayoutPanel.ColumnCount = 1;
            Mian_TableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            Mian_TableLayoutPanel.Controls.Add(tableLayoutPanel1, 0, 0);
            Mian_TableLayoutPanel.Controls.Add(tabControl1, 0, 1);
            Mian_TableLayoutPanel.Dock = DockStyle.Fill;
            Mian_TableLayoutPanel.Location = new Point(0, 0);
            Mian_TableLayoutPanel.Name = "Mian_TableLayoutPanel";
            Mian_TableLayoutPanel.RowCount = 2;
            Mian_TableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 40F));
            Mian_TableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 60F));
            Mian_TableLayoutPanel.Size = new Size(395, 446);
            Mian_TableLayoutPanel.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));
            tableLayoutPanel1.Controls.Add(groupBox1, 0, 0);
            tableLayoutPanel1.Controls.Add(groupBox2, 1, 0);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(3, 3);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Size = new Size(389, 172);
            tableLayoutPanel1.TabIndex = 1;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(lbl_COMAvailable);
            groupBox1.Controls.Add(label7);
            groupBox1.Controls.Add(lbl_FinalTime);
            groupBox1.Controls.Add(lbl_CarNo);
            groupBox1.Controls.Add(lbl_CarName);
            groupBox1.Controls.Add(lbl_COMOccupy);
            groupBox1.Controls.Add(lbl_COMStatus);
            groupBox1.Controls.Add(lbl_ServerStatus);
            groupBox1.Controls.Add(label6);
            groupBox1.Controls.Add(label5);
            groupBox1.Controls.Add(label4);
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(label1);
            groupBox1.Dock = DockStyle.Fill;
            groupBox1.Location = new Point(3, 3);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(246, 166);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "状态";
            // 
            // lbl_COMAvailable
            // 
            lbl_COMAvailable.AutoSize = true;
            lbl_COMAvailable.Location = new Point(92, 77);
            lbl_COMAvailable.Name = "lbl_COMAvailable";
            lbl_COMAvailable.Size = new Size(20, 17);
            lbl_COMAvailable.TabIndex = 12;
            lbl_COMAvailable.Text = "无";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(18, 77);
            label7.Name = "label7";
            label7.Size = new Size(68, 17);
            label7.TabIndex = 11;
            label7.Text = "串口可用：";
            // 
            // lbl_FinalTime
            // 
            lbl_FinalTime.AutoSize = true;
            lbl_FinalTime.Location = new Point(92, 146);
            lbl_FinalTime.Name = "lbl_FinalTime";
            lbl_FinalTime.Size = new Size(26, 17);
            lbl_FinalTime.TabIndex = 10;
            lbl_FinalTime.Text = "xxx";
            // 
            // lbl_CarNo
            // 
            lbl_CarNo.AutoSize = true;
            lbl_CarNo.Location = new Point(92, 127);
            lbl_CarNo.Name = "lbl_CarNo";
            lbl_CarNo.Size = new Size(26, 17);
            lbl_CarNo.TabIndex = 9;
            lbl_CarNo.Text = "xxx";
            // 
            // lbl_CarName
            // 
            lbl_CarName.AutoSize = true;
            lbl_CarName.Location = new Point(92, 108);
            lbl_CarName.Name = "lbl_CarName";
            lbl_CarName.Size = new Size(26, 17);
            lbl_CarName.TabIndex = 8;
            lbl_CarName.Text = "xxx";
            // 
            // lbl_COMOccupy
            // 
            lbl_COMOccupy.AutoSize = true;
            lbl_COMOccupy.Location = new Point(92, 60);
            lbl_COMOccupy.Name = "lbl_COMOccupy";
            lbl_COMOccupy.Size = new Size(20, 17);
            lbl_COMOccupy.TabIndex = 7;
            lbl_COMOccupy.Text = "无";
            // 
            // lbl_COMStatus
            // 
            lbl_COMStatus.AutoSize = true;
            lbl_COMStatus.Location = new Point(92, 42);
            lbl_COMStatus.Name = "lbl_COMStatus";
            lbl_COMStatus.Size = new Size(20, 17);
            lbl_COMStatus.TabIndex = 6;
            lbl_COMStatus.Text = "无";
            // 
            // lbl_ServerStatus
            // 
            lbl_ServerStatus.AutoSize = true;
            lbl_ServerStatus.Location = new Point(92, 19);
            lbl_ServerStatus.Name = "lbl_ServerStatus";
            lbl_ServerStatus.Size = new Size(44, 17);
            lbl_ServerStatus.TabIndex = 5;
            lbl_ServerStatus.Text = "未连接";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(42, 146);
            label6.Name = "label6";
            label6.Size = new Size(44, 17);
            label6.TabIndex = 4;
            label6.Text = "时间：";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(42, 127);
            label5.Name = "label5";
            label5.Size = new Size(44, 17);
            label5.TabIndex = 3;
            label5.Text = "车牌：";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(42, 108);
            label4.Name = "label4";
            label4.Size = new Size(44, 17);
            label4.TabIndex = 1;
            label4.Text = "车主：";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(18, 60);
            label3.Name = "label3";
            label3.Size = new Size(68, 17);
            label3.TabIndex = 2;
            label3.Text = "串口占用：";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(18, 42);
            label2.Name = "label2";
            label2.Size = new Size(68, 17);
            label2.TabIndex = 1;
            label2.Text = "串口打开：";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(6, 19);
            label1.Name = "label1";
            label1.Size = new Size(80, 17);
            label1.TabIndex = 0;
            label1.Text = "服务器状态：";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(cb_Sound);
            groupBox2.Controls.Add(cb_Light);
            groupBox2.Controls.Add(label8);
            groupBox2.Controls.Add(cmb_ComPorts);
            groupBox2.Controls.Add(btn_CloseLight);
            groupBox2.Controls.Add(btn_TestLight);
            groupBox2.Dock = DockStyle.Fill;
            groupBox2.Location = new Point(255, 3);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(131, 166);
            groupBox2.TabIndex = 1;
            groupBox2.TabStop = false;
            groupBox2.Text = "控制";
            // 
            // cb_Sound
            // 
            cb_Sound.AutoSize = true;
            cb_Sound.Checked = true;
            cb_Sound.CheckState = CheckState.Checked;
            cb_Sound.Location = new Point(74, 55);
            cb_Sound.Name = "cb_Sound";
            cb_Sound.Size = new Size(51, 21);
            cb_Sound.TabIndex = 15;
            cb_Sound.Text = "声音";
            cb_Sound.UseVisualStyleBackColor = true;
            // 
            // cb_Light
            // 
            cb_Light.AutoSize = true;
            cb_Light.Checked = true;
            cb_Light.CheckState = CheckState.Checked;
            cb_Light.Enabled = false;
            cb_Light.Location = new Point(6, 55);
            cb_Light.Name = "cb_Light";
            cb_Light.Size = new Size(51, 21);
            cb_Light.TabIndex = 14;
            cb_Light.Text = "灯光";
            cb_Light.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(6, 24);
            label8.Name = "label8";
            label8.Size = new Size(35, 17);
            label8.TabIndex = 13;
            label8.Text = "串口:";
            // 
            // cmb_ComPorts
            // 
            cmb_ComPorts.FormattingEnabled = true;
            cmb_ComPorts.Location = new Point(42, 18);
            cmb_ComPorts.Name = "cmb_ComPorts";
            cmb_ComPorts.Size = new Size(83, 25);
            cmb_ComPorts.TabIndex = 3;
            cmb_ComPorts.SelectedIndexChanged += cmb_ComPorts_SelectedIndexChanged;
            // 
            // btn_CloseLight
            // 
            btn_CloseLight.Location = new Point(6, 127);
            btn_CloseLight.Name = "btn_CloseLight";
            btn_CloseLight.Size = new Size(119, 33);
            btn_CloseLight.TabIndex = 1;
            btn_CloseLight.Text = "关闭灯光";
            btn_CloseLight.UseVisualStyleBackColor = true;
            btn_CloseLight.Click += btn_CloseLight_Click;
            // 
            // btn_TestLight
            // 
            btn_TestLight.Location = new Point(6, 85);
            btn_TestLight.Name = "btn_TestLight";
            btn_TestLight.Size = new Size(119, 36);
            btn_TestLight.TabIndex = 0;
            btn_TestLight.Text = "测试灯光";
            btn_TestLight.UseVisualStyleBackColor = true;
            btn_TestLight.Click += btn_TestLight_Click;
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Controls.Add(tabPage3);
            tabControl1.Controls.Add(tbpg_Settings);
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.Location = new Point(3, 181);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(389, 262);
            tabControl1.TabIndex = 2;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(dgv_AllEntryRecords);
            tabPage1.Location = new Point(4, 26);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(381, 232);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "车辆进入";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // dgv_AllEntryRecords
            // 
            dgv_AllEntryRecords.AllowUserToAddRows = false;
            dgv_AllEntryRecords.AllowUserToDeleteRows = false;
            dgv_AllEntryRecords.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgv_AllEntryRecords.Dock = DockStyle.Fill;
            dgv_AllEntryRecords.Location = new Point(3, 3);
            dgv_AllEntryRecords.Name = "dgv_AllEntryRecords";
            dgv_AllEntryRecords.ReadOnly = true;
            dgv_AllEntryRecords.RowHeadersVisible = false;
            dgv_AllEntryRecords.Size = new Size(375, 226);
            dgv_AllEntryRecords.TabIndex = 0;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(dgv_BlacklistEntryRecords);
            tabPage2.Location = new Point(4, 26);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(381, 232);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "进入记录";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // dgv_BlacklistEntryRecords
            // 
            dgv_BlacklistEntryRecords.AllowUserToAddRows = false;
            dgv_BlacklistEntryRecords.AllowUserToDeleteRows = false;
            dgv_BlacklistEntryRecords.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgv_BlacklistEntryRecords.Dock = DockStyle.Fill;
            dgv_BlacklistEntryRecords.Location = new Point(3, 3);
            dgv_BlacklistEntryRecords.Name = "dgv_BlacklistEntryRecords";
            dgv_BlacklistEntryRecords.ReadOnly = true;
            dgv_BlacklistEntryRecords.RowHeadersVisible = false;
            dgv_BlacklistEntryRecords.Size = new Size(375, 226);
            dgv_BlacklistEntryRecords.TabIndex = 0;
            // 
            // tabPage3
            // 
            tabPage3.Controls.Add(dgv_Blacklist);
            tabPage3.Location = new Point(4, 26);
            tabPage3.Name = "tabPage3";
            tabPage3.Padding = new Padding(3);
            tabPage3.Size = new Size(381, 232);
            tabPage3.TabIndex = 2;
            tabPage3.Text = "白名单";
            tabPage3.UseVisualStyleBackColor = true;
            // 
            // dgv_Blacklist
            // 
            dgv_Blacklist.AllowUserToAddRows = false;
            dgv_Blacklist.AllowUserToDeleteRows = false;
            dgv_Blacklist.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgv_Blacklist.Dock = DockStyle.Fill;
            dgv_Blacklist.Location = new Point(3, 3);
            dgv_Blacklist.Name = "dgv_Blacklist";
            dgv_Blacklist.ReadOnly = true;
            dgv_Blacklist.RowHeadersVisible = false;
            dgv_Blacklist.Size = new Size(375, 226);
            dgv_Blacklist.TabIndex = 0;
            // 
            // tbpg_Settings
            // 
            tbpg_Settings.Controls.Add(bt_ComOpen);
            tbpg_Settings.Controls.Add(tb_Port);
            tbpg_Settings.Controls.Add(tb_ServerURL);
            tbpg_Settings.Controls.Add(label10);
            tbpg_Settings.Controls.Add(label9);
            tbpg_Settings.Location = new Point(4, 26);
            tbpg_Settings.Name = "tbpg_Settings";
            tbpg_Settings.Padding = new Padding(3);
            tbpg_Settings.Size = new Size(381, 232);
            tbpg_Settings.TabIndex = 3;
            tbpg_Settings.Text = "设置";
            tbpg_Settings.UseVisualStyleBackColor = true;
            // 
            // bt_ComOpen
            // 
            bt_ComOpen.Location = new Point(186, 39);
            bt_ComOpen.Name = "bt_ComOpen";
            bt_ComOpen.Size = new Size(94, 32);
            bt_ComOpen.TabIndex = 4;
            bt_ComOpen.Text = "打开/关闭串口";
            bt_ComOpen.UseVisualStyleBackColor = true;
            bt_ComOpen.Click += bt_ComOpen_Click;
            // 
            // tb_Port
            // 
            tb_Port.Location = new Point(80, 44);
            tb_Port.Name = "tb_Port";
            tb_Port.Size = new Size(100, 23);
            tb_Port.TabIndex = 3;
            tb_Port.Text = "9600";
            // 
            // tb_ServerURL
            // 
            tb_ServerURL.Location = new Point(80, 9);
            tb_ServerURL.Name = "tb_ServerURL";
            tb_ServerURL.Size = new Size(295, 23);
            tb_ServerURL.TabIndex = 2;
            tb_ServerURL.Text = "ws://192.168.11.24:9096/ws-push/topic/messages";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new Point(15, 47);
            label10.Name = "label10";
            label10.Size = new Size(59, 17);
            label10.TabIndex = 1;
            label10.Text = "串口端口:";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(3, 12);
            label9.Name = "label9";
            label9.Size = new Size(71, 17);
            label9.TabIndex = 0;
            label9.Text = "服务器地址:";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(395, 446);
            Controls.Add(Main_Panel);
            Name = "MainForm";
            Text = "车辆声光报警系统";
            Load += MainForm_Load;
            Main_Panel.ResumeLayout(false);
            Mian_TableLayoutPanel.ResumeLayout(false);
            tableLayoutPanel1.ResumeLayout(false);
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgv_AllEntryRecords).EndInit();
            tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgv_BlacklistEntryRecords).EndInit();
            tabPage3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgv_Blacklist).EndInit();
            tbpg_Settings.ResumeLayout(false);
            tbpg_Settings.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel Main_Panel;
        private TableLayoutPanel Mian_TableLayoutPanel;
        private GroupBox groupBox1;
        private TableLayoutPanel tableLayoutPanel1;
        private GroupBox groupBox2;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private TabPage tabPage3;
        private Label label2;
        private Label label1;
        private Label label3;
        private Label label6;
        private Label label5;
        private Label label4;
        private Button btn_CloseLight;
        private Button btn_TestLight;
        private ComboBox cmb_ComPorts;
        private Label lbl_FinalTime;
        private Label lbl_CarNo;
        private Label lbl_CarName;
        private Label lbl_COMOccupy;
        private Label lbl_COMStatus;
        private Label lbl_ServerStatus;
        private DataGridView dgv_AllEntryRecords;
        private DataGridView dgv_BlacklistEntryRecords;
        private DataGridView dgv_Blacklist;
        private Label label7;
        private Label lbl_COMAvailable;
        private Label label8;
        private CheckBox cb_Sound;
        private CheckBox cb_Light;
        private TabPage tbpg_Settings;
        private Label label10;
        private Label label9;
        private TextBox tb_Port;
        private TextBox tb_ServerURL;
        private Button bt_ComOpen;
    }
}
