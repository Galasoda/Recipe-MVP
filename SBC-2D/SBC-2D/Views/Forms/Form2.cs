using SBC_2D.Infrastructures;
using SBC_2D.Infrastructures.Recipe;
using SBC_2D.Presenters;
using SBC_2D.Servicies;
using SBC_2D.Shared;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SBC_2D.Views
{
    public partial class Form2 : Form, IRecipeView
    {
        // ── 事件（使用者意圖）────────────────────────────────
        public event EventHandler Initialized;
        public event EventHandler<RecipeManageAction> ActionRequested;   
        public event EventHandler<string> ActionConfirmed;
        public event EventHandler ActionCancelled;
        public event EventHandler ToggleEditModeRequested;
        public event EventHandler<bool> MapModeBypassChanged;
        public event EventHandler<bool> UpperBrBypassChanged;
        public event EventHandler<bool> LowerBrBypassChanged;
        public event EventHandler<bool> LdsBypassChanged;
        public event EventHandler<string> ModelNameSelectChanged;

        public Form2()
        {
            InitializeComponent();
            ApplyTheme();
        }

        // ── Form Events ───────────────────────────────────────

        private void Form2_Load(object sender, EventArgs e)
            => Initialized?.Invoke(this, EventArgs.Empty);

        // ── Button Events（只發事件，不做決定）───────────────

        private void ComboBoxRecipes_SelectedIndexChanged(object sender, EventArgs e)
            => ModelNameSelectChanged?.Invoke(sender, comboBoxRecipes.SelectedItem.ToString());
        private void ButtonToggleEditMode_Click(object sender, EventArgs e)
            => ToggleEditModeRequested?.Invoke(this, EventArgs.Empty);

        private void ButtonRecipeActionRequest_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            RecipeManageAction action = RecipeManageAction.Nothing;
            if (button == buttonCanSelectRecipe)
                action = RecipeManageAction.Open;
            else if (button == buttonCanSaveRecipe)
                action = RecipeManageAction.Save;
            else if (button == buttonCanSaveasRecipe)
                action = RecipeManageAction.SaveAs;
            else if (button == buttonCanDeleteRecipe)
                action = RecipeManageAction.Delete;
            if (action != RecipeManageAction.Nothing)
                ActionRequested?.Invoke(this, action);
        }

        private void ButtonConfirmAction_Click(object sender, EventArgs e)
            => ActionConfirmed?.Invoke(this, comboBoxRecipes.Text);

        private void ButtonCancelAction_Click(object sender, EventArgs e)
            => ActionCancelled?.Invoke(this, EventArgs.Empty);

        public void SetEditMode(bool isEditing)
        {
            SuspendLayout();
            foreach (GroupBox gb in Controls.OfType<GroupBox>())
            {
                if (gb != groupBoxRecipeManager)
                    gb.Enabled = isEditing;
            }
            ResumeLayout();

            buttonSwitchEditOrView.Text = isEditing ? "V" : "E";
        }

        public void SetViewMode(RecipeManageAction action)
        {
            bool isIdle = action == RecipeManageAction.Nothing;
            bool isSave = action == RecipeManageAction.Save;
            bool isSaveAs = action == RecipeManageAction.SaveAs;
            bool needCombo = !isIdle && !isSave;
            labelRecipeName.Visible = !needCombo;
            comboBoxRecipes.Visible = needCombo;
            comboBoxRecipes.Enabled = needCombo;
            comboBoxRecipes.DropDownStyle = isSaveAs
                ? ComboBoxStyle.DropDown
                : ComboBoxStyle.DropDownList;
            flowLayoutPanelRecipeManageCtrls.Visible = isIdle;
            flowLayoutPanelRecipeManageCtrls.BringToFront();
            flowLayoutPanelConfirmCancel.Visible = !isIdle;
            if (!isIdle)
                labelRecipeMamageAction.Text = action.ToString();
        }

        public void ShowMessageBox(string message)
        {
        }

        public void ShowRecipe(Recipe recipe)
        {
            if (recipe == null) return;
            labelRecipeName.Text = recipe.Name;
            checkBoxMapModeBypass.Checked = recipe.IsMapModeBypass;
            checkBoxUpperBrBypass.Checked = recipe.IsUpperBrBypass;
            checkBoxLowerBrBypass.Checked = recipe.IsLowerBrBypass;
            checkBoxLdsBypass.Checked = recipe.IsLdsBypass;
            textBoxPcbThickness.Text = recipe.Thickness.ToString();
            textBoxThicknessTolrence.Text = recipe.ThicknessPosTolerance.ToString();
            labelPcbMaxThickness.Text = (recipe.Thickness + recipe.ThicknessPosTolerance).ToString();
            textBoxThicknessZeroBias.Text = recipe.ThicknessZeroBias.ToString();
            textBoxSubstrateBlockX.Text = recipe.PcbBlockX.ToString();
            textBoxSubstrateBlockY.Text = recipe.PcbBlockY.ToString();
            textBoxSubstrateBlockNumX.Text = recipe.PcbBlocksX.ToString();
            textBoxSubstrateBlockNumY.Text = recipe.PcbBlocksY.ToString();
            radioButtonSinglePcb.Checked = recipe.PcbCount == 1;
            radioButtonDualPcb.Checked = recipe.PcbCount == 2;
            checkBoxRotate.Checked = recipe.IsPcbRotate;
            comboBoxRecipes.SelectedItem = recipe.Name;
        }

        private void CheckBoxMapModeBypass_CheckedChanged(object sender, EventArgs e)
            => MapModeBypassChanged?.Invoke(this, checkBoxMapModeBypass.Checked);
        private void CheckBoxUpperBrBypass_CheckedChanged(object sender, EventArgs e)
            => UpperBrBypassChanged?.Invoke(this, checkBoxUpperBrBypass.Checked);
        private void CheckBoxLowerBrBypass_CheckedChanged(object sender, EventArgs e)
            => LowerBrBypassChanged?.Invoke(this, checkBoxLowerBrBypass.Checked);
        private void CheckBoxLdsBypass_CheckedChanged(object sender, EventArgs e)
            => LdsBypassChanged?.Invoke(this, checkBoxLdsBypass.Checked);

        public void SetSelectedName(string name)
        {
            comboBoxRecipes.SelectedItem = name;
            comboBoxRecipes.Text = name;
        }

        public void ShowRecipeNames(IEnumerable<string> names)
        {
            comboBoxRecipes.Items.Clear();
            comboBoxRecipes.Items.AddRange(names.ToArray());
        }

        public void RemoveRecipeName(string name)
        {
            comboBoxRecipes.Items.Remove(name);
            if (comboBoxRecipes.Items.Count > 0)
                comboBoxRecipes.SelectedIndex = 0;
        }

        private void ApplyTheme()
        {
            AppTheme.ApplyForm(this);

            ApplyButtonTheme(flowLayoutPanelRecipeManageCtrls);
            ApplyButtonTheme(groupBoxThicknessSetting);
            ApplyButtonTheme(flowLayoutPanelConfirmCancel);

            AppTheme.ApplyComboBox(panelSelectRecipe, comboBoxRecipes);
            AppTheme.ApplyButton(buttonGetPcbThickness);
            AppTheme.ApplyGroupBox(groupBoxRecipeManager);
            AppTheme.ApplyGroupBox(groupBoxSubstrateSetting);
            AppTheme.ApplyGroupBox(groupBoxBypassSetting);
            AppTheme.ApplyGroupBox(groupBoxThicknessSetting);
            AppTheme.ApplyTextbox(panel5, textBoxPcbThickness);
            AppTheme.ApplyTextbox(panel6, textBoxThicknessTolrence);
        }

        private void ApplyButtonTheme(Control container)
        {
            foreach (Button btn in container.Controls.OfType<Button>())
                AppTheme.ApplyButton(btn);
        }
    }
}