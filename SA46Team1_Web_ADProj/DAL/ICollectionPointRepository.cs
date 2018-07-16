using SA46Team1_Web_ADProj.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SA46Team1_Web_ADProj.DAL
{
    interface ICollectionPointRepository : IDisposable
    {
        IEnumerable<CollectionPoint> GetCollectionPoints();

        CollectionPoint GetCollectionPointById(string collectionId);

        void InsertCollectionPoint(CollectionPoint collectionPoint);

        void DeleteCollectionPoint(string collectionId);

        void UpdateCollectionPoint(CollectionPoint collectionPoint);

        void Save();

    }
}
