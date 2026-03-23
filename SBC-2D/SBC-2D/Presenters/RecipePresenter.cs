using Dapper.FluentMap.Mapping;
using SBC_2D.Domain.Servicies;
using SBC_2D.Infrastructures.Recipe;
using SBC_2D.Servicies;
using SBC_2D.Shared;
using SBC_2D.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Windows.Forms;

namespace SBC_2D.Presenters
{
    public class RecipePresenter
    {
        private readonly RecipeService _recipeService;
        private readonly IniService _iniService;
        private readonly IRecipeView _recipeView;
        private bool _isOnEdit;
        private Recipe _editedRecipe;
        private Recipe _currentRecipe;
        private List<string> _allRecipeNames;
        private string _selectedName;
        public RecipeManageAction ManageAction { get; private set; }

        // Events
        public event Action<Recipe> RecipeChanged;
        public event Action<List<string>> Initialized;
        public event Action<bool> OnValueChanged;

        public RecipePresenter(RecipeService recipeService, IniService iniService, IRecipeView recipeView)
        {
            _recipeService = recipeService;
            _iniService = iniService;
            _recipeView = recipeView;
            _editedRecipe = new Recipe();
            _currentRecipe = new Recipe();
            _allRecipeNames = new List<string>();
        }

        public void Initialize()
        {
            _recipeView.Initialized += RecipeView_Initialized; ;
            _recipeView.ToggleEditModeRequested += RecipeView_ToggleEditMode;
            _recipeView.ActionRequested += RecipeView_ActionRequested;
            _recipeView.ActionConfirmed += RecipeView_ActionConfirmed;
            _recipeView.ActionCancelled += RecipeView_ActionCancelled;
            _recipeView.MapModeBypassChanged += RecipeView_MapModeBypassChanged;
            _recipeView.UpperBrBypassChanged += RecipeView_UpperBrBypassChanged;
            _recipeView.LowerBrBypassChanged += RecipeView_LowerBrBypassChanged;
            _recipeView.LdsBypassChanged += RecipeView_LdsBypassChanged;
            _recipeView.ModelNameSelectChanged += RecipeView_ModelNameSelectChanged;
            _isOnEdit = false;
            _recipeView.SetEditMode(_isOnEdit);
            ManageAction = RecipeManageAction.Nothing;
            _recipeView.SetViewMode(ManageAction);
        }

        private void RecipeView_Initialized(object sender, EventArgs e)
        {
            List<string> names = _recipeService.GetAllNames();
            string name = _iniService.GetName() ?? string.Empty;
            Recipe recipe = _recipeService.Get(name);
            _currentRecipe = recipe.DeepClone();
            _editedRecipe = recipe.DeepClone();
            _allRecipeNames = names;
            _recipeView.ShowRecipeNames(names);
            _recipeView.ShowRecipe(recipe);
            RecipeChanged?.Invoke(recipe);
        }

        private void RecipeView_LdsBypassChanged(object sender, bool e)
            => _editedRecipe.IsLdsBypass = e;

        private void RecipeView_LowerBrBypassChanged(object sender, bool e)
        {
            _editedRecipe.IsLowerBrBypass = e;
            if (e)
            {
                _editedRecipe.IsMapModeBypass = false;
                _editedRecipe.IsUpperBrBypass = false;
                _recipeView.ShowRecipe(_editedRecipe);
            }
        }

        private void RecipeView_UpperBrBypassChanged(object sender, bool e)
        {
            _editedRecipe.IsUpperBrBypass = e;
            if (e)
            {
                _editedRecipe.IsMapModeBypass = false;
                _editedRecipe.IsLowerBrBypass = false;
                _recipeView.ShowRecipe(_editedRecipe);
            }
        }

        private void RecipeView_MapModeBypassChanged(object sender, bool e)
        {
            _editedRecipe.IsMapModeBypass = e;
            if (e)
            {
                _editedRecipe.IsUpperBrBypass = false;
                _editedRecipe.IsLowerBrBypass = false;
                _recipeView.ShowRecipe(_editedRecipe);
            }
        }

        private void RecipeView_ActionRequested(object sender, RecipeManageAction action)
        {
            ManageAction = action;
            _recipeView.SetViewMode(action);
            if (action == RecipeManageAction.Delete ||
                action == RecipeManageAction.Open)
                _recipeView.SetEditMode(false);
        }

        private void RecipeView_ActionConfirmed(object sender, string name)
        {
            string message = string.Empty;
            bool success = false;

            switch (ManageAction)
            {
                case RecipeManageAction.Open:
                    success = Load(name);
                    break;

                case RecipeManageAction.Save:
                    success = Save(out message);
                    break;

                case RecipeManageAction.SaveAs:
                    success = SaveNew(name, out message);
                    break;

                case RecipeManageAction.Delete:
                    success = Delete(name, out message);
                    break;

                default:
                    break;
            }

            if (!string.IsNullOrEmpty(message))
                _recipeView.ShowMessageBox(message);

            ManageAction = RecipeManageAction.Nothing;
            _recipeView.SetViewMode(RecipeManageAction.Nothing);
        }

        private void RecipeView_ActionCancelled(object sender, EventArgs e)
            => CancelAction();

        private void RecipeView_ToggleEditMode(object sender, EventArgs e)
        {
            _isOnEdit = !_isOnEdit;
            _recipeView.SetEditMode(_isOnEdit);
        }
        private void RecipeView_ModelNameSelectChanged(object sender, string e)
            => _selectedName = e;


        // ── Recipe Operations ────────────────────────────────────

        public bool Load(string name)
        {
            Recipe recipe = _recipeService.Get(name);
            _recipeView.ShowRecipe(recipe);
            RecipeChanged?.Invoke(recipe);
            return true;
        }

        public bool Save(out string message)
        {
            bool isSaved = _recipeService.Save(_editedRecipe, out message);
            if (isSaved)
            {
                _currentRecipe = _editedRecipe.DeepClone();
                RecipeChanged?.Invoke(_currentRecipe);
            }
            return isSaved;
        }

        public bool SaveNew(string newName, out string message)
        {
            Recipe newRecipe = _editedRecipe.DeepClone();
            newRecipe.Name = newName;
            bool isCreated = _recipeService.Create(newRecipe, out message);
            if (isCreated)
            {
                _allRecipeNames.Add(newName);
                _currentRecipe = newRecipe.DeepClone();
                _recipeView.ShowRecipeNames(_allRecipeNames);
                RecipeChanged?.Invoke(newRecipe);
            }
            return isCreated;
        }

        public bool Delete(string name, out string message)
        {
            bool isDeleted = _recipeService.Delete(name, out message);
            if (isDeleted)
            {
                _recipeView.RemoveRecipeName(name);
                if (_allRecipeNames.Any())
                    Load(_allRecipeNames.First());
            }
            return isDeleted;
        }

        public void RequestAction(RecipeManageAction action)
        {
            ManageAction = action;
            _recipeView.SetViewMode(action);
        }

        public void CancelAction()
        {
            ManageAction = RecipeManageAction.Nothing;
            _recipeView.SetViewMode(RecipeManageAction.Nothing);
            _recipeView.SetEditMode(_isOnEdit);
            if (_selectedName != _currentRecipe.Name)
                _recipeView.SetSelectedName(_currentRecipe.Name);

        }
    }
}