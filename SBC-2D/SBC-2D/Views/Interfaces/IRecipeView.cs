using SBC_2D.Infrastructures.Recipe;
using SBC_2D.Shared;
using System;
using System.Collections.Generic;

namespace SBC_2D.Views
{
    public interface IRecipeView
    {
        event EventHandler Initialized;
        event EventHandler ToggleEditModeRequested;
        event EventHandler<RecipeManageAction> ActionRequested;
        event EventHandler<string> ActionConfirmed;
        event EventHandler ActionCancelled;
        event EventHandler<bool> MapModeBypassChanged;
        event EventHandler<bool> UpperBrBypassChanged;
        event EventHandler<bool> LowerBrBypassChanged;
        event EventHandler<bool> LdsBypassChanged;
        event EventHandler<string> ModelNameSelectChanged;

        void ShowRecipeNames(IEnumerable<string> names);
        void ShowRecipe(Recipe recipe);
        void SetEditMode(bool isEditing);
        void SetViewMode(RecipeManageAction action);
        void SetSelectedName(string name);
        void RemoveRecipeName(string name);
        void ShowMessageBox(string message);
    }
}