using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Element.Reveal.Meg.Discipline.ITR
{
    public class CDocRepository
    {
        public Windows.Storage.StorageFolder BaseRepositoryPath { get; set; }
        public string BaseRepositoryName { get; set; }
        private WinAppLibrary.Utilities.Helper helper;

        public CDocRepository(Windows.Storage.StorageFolder _path, string _filename)
        {
            BaseRepositoryPath = _path;
            BaseRepositoryName = _filename;
        }

        public async Task<object> GetDoc(string _key)
        {
            Dictionary<string, object> itrList = await Load();
            return itrList[_key];
        }

        private async Task<Dictionary<string, object>> Load()
        {
            try
            {
                var stream = await helper.GetFileStream(BaseRepositoryPath, BaseRepositoryName);
                return await helper.EncryptDeserializeFrom<Dictionary<string, object>>(stream);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<bool> Update(string _dockey, string _key, string _value)
        {
            try
            {
                var obj = await GetDoc(_dockey);
                //var xmlstream = helper.EncryptSerializeTo <Dictionary<string, object>>(_data);
                //await helper.SaveFileStream(BaseRepositoryPath, BaseRepositoryName, xmlstream);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
