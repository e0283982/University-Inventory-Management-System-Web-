﻿using SA46Team1_Web_ADProj.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//-----------------------------------------------------------------
//   Authors: Chirag Shetty
//-----------------------------------------------------------------

namespace SA46Team1_Web_ADProj.DAL
{    
    interface ICategoryRepository : IDisposable
    {
        IEnumerable<Category> GetCategories();

        Category GetCategoryById(int categoryId);

        void InsertCategory(Category category);

        void DeleteCategory(int categoryId);

        void UpdateCategory(Category category);

        void Save();
    }
}
