using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using SA46Team1_Web_ADProj.Models;

//-----------------------------------------------------------------
//   Authors: Chirag Shetty
//-----------------------------------------------------------------

namespace SA46Team1_Web_ADProj.DAL
{    
    public class CategoryRepositoryImpl : ICategoryRepository, IDisposable
    {
        private SSISdbEntities context;
        public CategoryRepositoryImpl(SSISdbEntities context)
        {
            this.context = context;
        }
        public void DeleteCategory(int categoryId)
        {
            Category category = context.Categories.Find(categoryId);
            context.Categories.Remove(category);
        }
        public Category GetCategoryById(int categoryId)
        {
            return context.Categories.Find(categoryId);
        }
        public IEnumerable<Category> GetCategories()
        {
            return context.Categories.ToList();
        }
        public void InsertCategory(Category category)
        {
            context.Categories.Add(category);
        }
        public void UpdateCategory(Category category)
        {
            context.Entry(category).State = EntityState.Modified;
        }
        public void Save()
        {
            context.SaveChanges();
        }
        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}