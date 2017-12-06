namespace Task4_List
{
    partial class Form1
    {
        /// <summary>
        /// Требуется переменная конструктора.
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
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.button_Evaluate = new System.Windows.Forms.Button();
            this.richTextBox_output = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // button_Evaluate
            // 
            this.button_Evaluate.Location = new System.Drawing.Point(610, 44);
            this.button_Evaluate.Name = "button_Evaluate";
            this.button_Evaluate.Size = new System.Drawing.Size(75, 23);
            this.button_Evaluate.TabIndex = 0;
            this.button_Evaluate.Text = "Evaluate";
            this.button_Evaluate.UseVisualStyleBackColor = true;
            this.button_Evaluate.Click += new System.EventHandler(this.button_Evaluate_Click);
            // 
            // richTextBox_output
            // 
            this.richTextBox_output.Location = new System.Drawing.Point(29, 102);
            this.richTextBox_output.Name = "richTextBox_output";
            this.richTextBox_output.Size = new System.Drawing.Size(685, 399);
            this.richTextBox_output.TabIndex = 1;
            this.richTextBox_output.Text = "";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(770, 541);
            this.Controls.Add(this.richTextBox_output);
            this.Controls.Add(this.button_Evaluate);
            this.Name = "Form1";
            this.Text = "TrackableList";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button_Evaluate;
        //private static System.Windows.Forms.RichTextBox richTextBox_out;
        private System.Windows.Forms.RichTextBox richTextBox_output;
    }
}

