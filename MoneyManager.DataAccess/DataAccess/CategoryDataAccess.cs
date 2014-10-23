using MoneyManager.DataAccess.Model;
using MoneyManager.Foundation;
using MoneyManager.Models;
using MoneyManager.Src;
using PropertyChanged;
using System.Collections.ObjectModel;
using System.Linq;

namespace MoneyManager.DataAccess
{
    [ImplementPropertyChanged]
    internal class CategoryDataAccess : AbstractDataAccess<Category>
    {
        public ObservableCollection<Category> AllCategories { get; set; }

        public Category SelectedCategory { get; set; }

        public CategoryDataAccess()
        {
            LoadList();
        }

        protected override void SaveToDb(Category category)
        {
            if (AllCategories == null)
            {
                AllCategories = new ObservableCollection<Category>();
            }

            //TODO: Refactor
            //category.Id = Utilities.GetMaxId();

            AllCategories.Add(category);
            AllCategories = new ObservableCollection<Category>(AllCategories.OrderBy(x => x.Name));

            var roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
            roamingSettings.Values[category.Id.ToString()] = category.Name;
        }

        protected override void DeleteFromDatabase(Category category)
        {
            var roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
            roamingSettings.Values.Remove(category.Id.ToString());

            AllCategories.Remove(category);
        }

        protected override void GetListFromDb()
        {
            var roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;

            AllCategories = new ObservableCollection<Category>(roamingSettings.Values
                .OrderBy(x => x.Value)
                .Select(x => new Category
                {
                    Id = int.Parse(x.Key),
                    Name = x.Value.ToString()
                }).ToList());
        }

        protected override void UpdateItem(Category category)
        {
            var roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
            roamingSettings.Values[category.Id.ToString()] = category.Name;
        }

        public void DeleteAll()
        {
            var roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
            roamingSettings.Values.Clear();

            AllCategories.Clear();
        }
    }
}