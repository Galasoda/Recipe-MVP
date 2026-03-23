using Dapper.FluentMap;
using SBC_2D.Domain.Servicies;
using SBC_2D.Infrastructures.Ini;
using SBC_2D.Infrastructures.Recipe;
using SBC_2D.Presenters;
using SBC_2D.Servicies;
using SBC_2D.Shared;
using SBC_2D.Views;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace SBC_2D
{
    static class Program
    {
        /// <summary>
        /// 應用程式的主要進入點。
        /// </summary>
        /// 

        [STAThread]

        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Batteries.Init();
            IniStore iniStore = new IniStore();
            IniService iniService = new IniService(iniStore);
            Setup setup = iniService.GetSetup();
            FluentMapper.Initialize(config =>
            {
                config.AddMap(new RecipeMap());
            });

            RecipeDao recipeDao = new RecipeDao(iniStore.Setup.PathConfig.SqLiteFile);
            RecipeService recipeService = new RecipeService(iniService, recipeDao);

            var form2 = new Form2();
            RecipePresenter recipePresenter = new RecipePresenter(recipeService, iniService, form2);
            recipePresenter.Initialize();
            Application.Run(form2);
        }
    }
}
