using SA46Team1_Web_ADProj.Models;
using System;
using System.Collections.Generic;

namespace SA46Team1_Web_ADProj.DAL
{
    interface IBinRepository: IDisposable
    {
        IEnumerable<Bin> GetBins();
        Bin GetBinById(int binId);
        void InsertBin(Bin bin);
        void DeleteBin(int binId);
        void UpdateBin(Bin bin);
        void Save();
    }
}
