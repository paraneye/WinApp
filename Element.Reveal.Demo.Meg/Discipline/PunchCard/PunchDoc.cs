using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Element.Reveal.Meg.Lib;
using Element.Reveal.Meg.Discipline.ITR;

namespace Element.Reveal.Meg.Discipline.PunchCard
{
   
    public class PunchDoc
    {
        public EStatusType Status { get; set; }
        public RevealProjectSvc.PunchDTOSet DTO { get; set; }
        public RevealProjectSvc.QaqcformDTO QAQCDTO { get; set; }
        public List<KeyValue> ExtraData { get; set; }

        /// <summary>
        /// Contstruction
        /// </summary>
        public PunchDoc()
        {
            ExtraData = new List<KeyValue>();
            DTO = new RevealProjectSvc.PunchDTOSet();
            QAQCDTO = new RevealProjectSvc.QaqcformDTO();
            //DTO.qaqcformDTOS[0].QaqcfromDetails = new List<RevealProjectSvc.QaqcformdetailDTO>();
        }

//        private void UpdateObject(List<RevealProjectSvc.QaqcformdetailDTO> _data, List<RevealProjectSvc.QaqcformdetailDTO> _update)
//        {
//            List<RevealProjectSvc.QaqcformdetailDTO> header = (from q in DTO.qaqcformdetailDTOS where q.InspectionLUID == QAQCGroup.Header select q)
//                .ToList<RevealProjectSvc.QaqcformdetailDTO>();
//            List<RevealProjectSvc.QaqcformdetailDTO> grid = (from q in DTO.qaqcformdetailDTOS where q.InspectionLUID == QAQCGroup.Grid select q)
//                .ToList<RevealProjectSvc.QaqcformdetailDTO>();

//            DTO.qaqcformdetailDTOS = new List<RevealProjectSvc.QaqcformdetailDTO>();
//            DTO.qaqcformdetailDTOS.AddRange(header);
//            if (_update != null)
//                DTO.qaqcformdetailDTOS.AddRange(_update);
//            else
//                DTO.qaqcformdetailDTOS.AddRange(grid);

//            foreach (RevealProjectSvc.QaqcformdetailDTO dto in _data)
//            {
//                if (dto.InspectionLUID == QAQCGroup.Header) continue;
//                if (dto.InspectionLUID == QAQCGroup.Grid) continue;

//                dto.QAQCFormID = DTO.qaqcformDTOS[0].QAQCFormID;
//                DTO.qaqcformdetailDTOS.Add(dto);
//            }

//            foreach (RevealProjectSvc.QaqcformdetailDTO dto in DTO.qaqcformdetailDTOS)
//            {
//                dto.DTOStatus = (int)((dto.QAQCFormDetailID != null && dto.QAQCFormDetailID > 0) ? WinAppLibrary.Utilities.RowStatus.Update
//                    : WinAppLibrary.Utilities.RowStatus.New);
//                if (dto.DateValue1 == DateTime.MinValue) dto.DateValue1 = WinAppLibrary.Utilities.Helper.DateTimeMinValue; // DateTime.Now;
//                if (dto.DateValue2 == DateTime.MinValue) dto.DateValue2 = WinAppLibrary.Utilities.Helper.DateTimeMinValue; // DateTime.Now;
//            }

//            this.DTO.qaqcformDTOS[0].DownloadStatus = (int)WinAppLibrary.Utilities.RowStatus.Update;
//        }
//        #region Public Method
//        /// <summary>
//        /// LocalStorage에 Save
//        /// </summary>
//        /// <param name="_data">사용자가 입력한 Data</param>
//        /// <returns>저장 성공여부(T/F)</returns>
//        public async Task<bool> Save(List<RevealProjectSvc.QaqcformdetailDTO> _data, Windows.Storage.StorageFolder _path, string _filename,
//            List<RevealProjectSvc.QaqcformdetailDTO> _update)
//        {
//            UpdateObject(_data, _update);
//            bool retValue = false;
//            WinAppLibrary.Utilities.Helper helper = new WinAppLibrary.Utilities.Helper();
//            try
//            {
//                //var xmlstream =  helper.EncryptHashSerializeTo<QAQCDoc>(this);
//                //var xmlstream = EncryptHashSerializeTo<QAQCDoc>(this);
//                var xmlstream = EncryptHashSerializeTo<RevealProjectSvc.QaqcformDTO>(this.DTO);
//                await helper.SaveFileStream(_path, _filename, xmlstream);
//                this.Status = EStatusType.Saved;
//                retValue = true;
//            }
//            catch (Exception e)
//            {
//                helper.ExceptionHandler(e, "Error Save User Data");
//                throw e;
//            }

//            return retValue;
//        }

//        /// <summary>
//        /// LocalStorage에 xml문서 삭제
//        /// </summary>
//        /// <param name="_path"></param>
//        /// <param name="_filename"></param>
//        /// <returns></returns>
//        public async Task<bool> Delete(Windows.Storage.StorageFolder _path, string _filename)
//        {
//            bool returnValue = false;
//            WinAppLibrary.Utilities.Helper helper = new WinAppLibrary.Utilities.Helper();
//            try
//            {
//               returnValue = await helper.DeleteFileStream(_path, _filename);
//            }
//            catch (Exception ex)
//            { 
//                helper.ExceptionHandler(ex, "Error Delete Doc");
//                throw ex;
//            }
//            return returnValue;
//        }
        
//        public Stream EncryptHashSerializeToLog<T>(object obj)
//        {
//            Stream retValue = null;
//            WinAppLibrary.Utilities.Helper helper = new WinAppLibrary.Utilities.Helper();
//            try
//            {
//                XmlSerializer serializer = new XmlSerializer(typeof(T));
//                using (StringWriter stringwriter = new StringWriter())
//                {
//                    using (System.Xml.XmlWriter xmlwriter = System.Xml.XmlWriter.Create(stringwriter))
//                    {
//                        serializer.Serialize(stringwriter, obj);
//                        retValue = new MemoryStream(Encoding.UTF8.GetBytes(stringwriter.ToString()));
//                    }
//                }
//            }
//            catch { }

//            return retValue;
//        }

//        private Stream EncryptHashSerializeTo<T>(object obj)
//        {
//            Stream retValue = null;
//            WinAppLibrary.Utilities.Helper helper = new WinAppLibrary.Utilities.Helper();
//            try
//            {
//                XmlSerializer serializer = new XmlSerializer(typeof(T));
//                using (StringWriter stringwriter = new StringWriter())
//                {
//                    using (System.Xml.XmlWriter xmlwriter = System.Xml.XmlWriter.Create(stringwriter))
//                    {
//                        serializer.Serialize(stringwriter, obj);
//                        string xml = WinAppLibrary.Utilities.Helper.Encrypt(stringwriter.ToString(), WinAppLibrary.Utilities.HashKey.Key_DrawingList);
//                        retValue = new MemoryStream(Encoding.UTF8.GetBytes(xml));
//                    }
//                }
//            }
//            catch(Exception ex) { }

//            return retValue;
//        }

        

//        /// <summary>
//        /// 사용자 데이터 Load(Local Storage)
//        /// </summary>
//        /// <param name="_path">저장 경로</param>
//        /// <returns>Load 성공 여부(T/F)</returns>
//        public async Task<bool> Load(Windows.Storage.StorageFolder _path, string _filename)
//        {
//            WinAppLibrary.Utilities.Helper helper = new WinAppLibrary.Utilities.Helper();
//            try
//            {
//                var stream = await helper.GetFileStream(_path, _filename);
//               //QAQCDoc _doc = await helper.EncryptDeserializeFrom<QAQCDoc>(stream);
//                DTO  = await helper.EncryptDeserializeFrom<RevealProjectSvc.WalkdownDTOSet>(stream);
//            //    this.ExtraData = _doc.ExtraData;
//                //this.DocNumber = _doc.DocNumber;
//             //   this.Status= _doc.Status;
//                //this.DTO.QaqcfromDetails = _doc.DTO.QaqcfromDetails;
//              //  this.DTO.QaqcfromDetails = doc.QaqcfromDetails;
//            }
//            catch (Exception e)
//            {
//                return false;
//            }
//            return true;
//        }

//        /// <summary>
//        /// WCF에 저장한 데이터 Push
//        /// </summary>
//        /// <returns>성공 여부(T/F)</returns>
//        public async Task<bool> Submit()
//        {
//            try
//            {
//                if (this.Status != EStatusType.ReadyToSubmit)
//                    return false;
//                List<RevealProjectSvc.QaqcformDTO> qaqcform =  await Gosubmit();
//                if (qaqcform.Count > 0)
//                    return true;
//                else return false;
                
//                //RevealProjectSvc.SaveQaqcformForSubmit(dddd, new List<QaqcFormDTO>{ this.DTO });
//                //TODO : Push Data
//            }
//            catch (Exception e)
//            {
//                return false;
//            }
//        }

//        public async Task<List<RevealProjectSvc.QaqcformDTO>> Gosubmit()
//        {
//            this.DTO.qaqcformDTOS[0].IsSubmitted = 1;  //0=Download / 1=Submit
//            this.DTO.qaqcformDTOS[0].UpdatedDate = DateTime.Now;
                
//            return await (new Lib.ServiceModel.ProjectModel()).SaveQaqcformForSubmit(new List<RevealProjectSvc.QaqcformDTO> { this.DTO});
//        }

        /// <summary>
        /// 사용자 입력값을 조사하여 Submit이 가능한 상태인지 Check
        /// </summary>
        /// <returns>처리 결과(T/F)</returns>
        public bool ReadyToSubmit()
        {
            this.Status = EStatusType.ReadyToSubmit;
            return true;
        }
//        #endregion

        #region Private Method
        private bool Validata()
        {
            return false;
        }
        #endregion
    }

    public enum EStatusType { Downloaded, Saved, ReadyToSubmit }

    public class KeyValue
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public KeyValue()
        {
            Key = ""; Value = "";
        }
    }
}

