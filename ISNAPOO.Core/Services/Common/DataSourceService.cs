using Data.Models.Common;
using Data.Models.Data.Common;
using Data.Models.Data.DOC;
using Data.Models.Data.SPPOO;
using ISNAPOO.Common.Constants;
using ISNAPOO.Common.HelperClasses;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Mapping;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.DOC;
using ISNAPOO.Core.ViewModels.SPPOO;
using Microsoft.EntityFrameworkCore;
using Syncfusion.DocIO.DLS;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ISNAPOO.Core.Services.Common
{
    public class DataSourceService : BaseService, IDataSourceService
    {
        private static readonly SemaphoreLocker _locker = new SemaphoreLocker();
        private readonly IRepository repository;


        public List<KeyValueVM> GetAllKeyValueList()
        {
            return AllKeyValueList;
        }

        public List<KeyType> GetAllKeyTypeList()
        {
            return AllKeyTypeList;
        }

        public List<Setting> GetAllSettingList()
        {
            return AllSettingList;
        }

        public List<AreaVM> GetAllAreasList()
        {
            return AllAreasList;
        }

        public List<ProfessionalDirectionVM> GetAllProfessionalDirectionsList()
        {
            return AllProfessionalDirectionsList;
        }

        public List<ProfessionVM> GetAllProfessionsList()
        {
            return AllProfessionsList;
        }

        public List<SpecialityVM> GetAllSpecialitiesList()
        {
            return AllSpecialitiesList;
        }

        public List<ERUVM> GetAllERUsList()
        {
            return AllERUSList;
        }


        public List<MenuNodeVM> GetAllMenuNode()
        {
            return AllMenuNodeList;
        }

        private static List<KeyValueVM> AllKeyValueList { get; set; }
        private static List<KeyType> AllKeyTypeList { get; set; }
        private static List<Setting> AllSettingList { get; set; }
        private static List<AreaVM> AllAreasList { get; set; }
        private static List<ProfessionalDirectionVM> AllProfessionalDirectionsList { get; set; }
        private static List<ProfessionVM> AllProfessionsList { get; set; }
        private static List<SpecialityVM> AllSpecialitiesList { get; set; }
        private static List<ERUVM> AllERUSList { get; set; }

        private static List<PolicyVM> AllPolicyList { get; set; }
        private static List<MenuNodeVM> AllMenuNodeList { get; set; }

        public DataSourceService(IRepository repository, IKeyValueService keyValueService, ISettingService settingService) : base(repository)
        {
            this.repository = repository;

            if (AllKeyValueList == null || AllKeyValueList.Count() == 0)
            {
                Task.Run(() => ReloadKeyValue()).Wait();

            }

            if (AllKeyTypeList == null || AllKeyTypeList.Count() == 0)
            {

                Task.Run(() => ReloadKeyType()).Wait();

            }

            if (AllSettingList == null || AllSettingList.Count() == 0)
            {
                Task.Run(() => ReloadSettings()).Wait();

            }

            if (AllAreasList == null || AllAreasList.Count() == 0)
            {
                Task.Run(() => ReloadAreas()).Wait();

            }

            if (AllProfessionalDirectionsList == null || AllProfessionalDirectionsList.Count() == 0)
            {
                Task.Run(() => ReloadProfessionalDirections()).Wait();

            }

            if (AllProfessionsList == null || AllProfessionsList.Count() == 0)
            {
                Task.Run(() => ReloadProfessions()).Wait();

            }

            if (AllSpecialitiesList == null || AllSpecialitiesList.Count() == 0)
            {
                Task.Run(() => ReloadSpecialities()).Wait();

            }

            if (AllERUSList == null || AllERUSList.Count() == 0)
            {
                Task.Run(() => ReloadERUs()).Wait();

            }

            if (AllPolicyList == null || AllPolicyList.Count() == 0)
            {

                Task.Run(() => ReloadPolicies()).Wait();

            }

            if (AllMenuNodeList == null || AllMenuNodeList.Count() == 0)
            {
                Task.Run(() => ReloadMenuNode()).Wait();
            }
        }

        public async Task<KeyValueVM> GetKeyValueByIdAsync(int? id)
        {
            KeyValueVM keyValue = AllKeyValueList.FirstOrDefault(k => k.IdKeyValue == id);
            return keyValue;
        }

        public async Task<Setting> GetSettingByIntCodeAsync(string settingIntCode)
        {
            if (AllSettingList == null || AllSettingList.Count() == 0)
            {
                await ReloadSettings();
            }

            Setting setting = AllSettingList.FirstOrDefault(s => s.SettingIntCode == settingIntCode);
            return setting;
        }




        public async Task<IEnumerable<KeyValueVM>> GetKeyValuesByKeyTypeIntCodeAsync(string keyTypeIntCode, int idKeyValue, bool addDefaultValue = false)
        {
            KeyType keyType = AllKeyTypeList.Where(x => x.KeyTypeIntCode == keyTypeIntCode).FirstOrDefault();

            if (keyType != null)
            {
                List<KeyValueVM> keyValues = AllKeyValueList.Where(x => x.IdKeyType == keyType.IdKeyType && x.IsActive).ToList();
                if (addDefaultValue)
                {
                    keyValues.Insert(0, new KeyValueVM() { IdKeyValue = 0, Name = GlobalConstants.NOT_SELECTED_LIST_VALUE_SHORT });
                }

                if (idKeyValue != 0 && keyValues.Count(x => x.IdKeyValue == idKeyValue) == 0)
                {
                    keyValues.Add(AllKeyValueList.FirstOrDefault(x => x.IdKeyValue == idKeyValue));


                }

                return keyValues.OrderByDescending(x => x.IsActive).ThenBy(x => x.Order);
            }

            return null;
        }

        public async Task<IEnumerable<KeyValueVM>> GetKeyValuesByKeyTypeIntCodeAsync(string keyTypeIntCode, bool addDefaultValue = false, bool allKeyValue = false)
        {
            KeyType keyType = AllKeyTypeList.Where(x => x.KeyTypeIntCode == keyTypeIntCode).FirstOrDefault();

            bool activeOnly = true;


            if (allKeyValue)
            {
                activeOnly = false;
            }

            if (keyType != null)
            {
                List<KeyValueVM> keyValues = AllKeyValueList.Where(x => x.IdKeyType == keyType.IdKeyType && (activeOnly ? x.IsActive : true)).ToList();
                if (addDefaultValue)
                {
                    keyValues.Insert(0, new KeyValueVM() { IdKeyValue = 0, Name = GlobalConstants.NOT_SELECTED_LIST_VALUE_SHORT });
                }

                return keyValues;
            }

            return null;
        }


        public async Task<KeyValueVM> GetKeyValueByIntCodeAsync(string keyTypeIntCode, string keyValueIntCode)
        {
            KeyType keyType = AllKeyTypeList.Where(x => x.KeyTypeIntCode == keyTypeIntCode).FirstOrDefault();
            KeyValueVM keyValue = AllKeyValueList.Where(x => x.KeyValueIntCode == keyValueIntCode && x.IdKeyType == keyType.IdKeyType && x.IsActive).FirstOrDefault();

            return keyValue;
        }

        public async Task<IEnumerable<KeyValueVM>> GetKeyValuesByListIdsAsync(List<int> ids)
        {
            List<KeyValueVM> keyValues = AllKeyValueList.Where(x => ids.Contains(x.IdKeyValue)).ToList();

            return keyValues;
        }

        public async Task ReloadSettings()
        {
            await _locker.LockAsync(async () =>
            {
                AllSettingList = new List<Setting>();
                AllSettingList = await this.repository.All<Setting>().ToListAsync();
            });
        }

        public async Task ReloadKeyValue()
        {
            await _locker.LockAsync(async () =>
            {
                AllKeyValueList = new List<KeyValueVM>();
                IQueryable<KeyValue> keyValues = this.repository.All<KeyValue>().Include(k => k.KeyType);
                AllKeyValueList = keyValues.To<KeyValueVM>().ToList();
            });
        }

        public async Task ReloadKeyType()
        {
            await _locker.LockAsync(async () =>
            {
                AllKeyTypeList = new List<KeyType>();
                AllKeyTypeList = this.repository.All<KeyType>().ToList();
            });
        }

        public async Task ReloadAreas()
        {
            await _locker.LockAsync(async () =>
            {
                AllAreasList = new List<AreaVM>();
                IQueryable<Area> areas = this.repository.AllReadonly<Area>();
                AllAreasList = areas.To<AreaVM>().ToList();
            });
        }

        public async Task ReloadProfessionalDirections()
        {
            await _locker.LockAsync(async () =>
            {
                AllProfessionalDirectionsList = new List<ProfessionalDirectionVM>();
                IQueryable<ProfessionalDirection> professionalDirections = this.repository.AllReadonly<ProfessionalDirection>();
                AllProfessionalDirectionsList = professionalDirections.To<ProfessionalDirectionVM>().ToList();
            });
        }

        public async Task ReloadProfessions()
        {
            await _locker.LockAsync(async () =>
            {
                AllProfessionsList = new List<ProfessionVM>();
                IQueryable<Profession> professions = this.repository.AllReadonly<Profession>();
                AllProfessionsList = professions.To<ProfessionVM>().ToList();
            });
        }

        public async Task ReloadSpecialities()
        {
            await _locker.LockAsync(async () =>
            {
                AllSpecialitiesList = new List<SpecialityVM>();
                IQueryable<Speciality> specialities = this.repository.AllReadonly<Speciality>();
                AllSpecialitiesList = specialities.To<SpecialityVM>().ToList();
            });
        }

        public async Task ReloadERUs()
        {
            await _locker.LockAsync(async () =>
            {
                IQueryable<ERU> erus = this.repository.AllReadonly<ERU>();
                var keyValues = this.GetAllKeyValueList();

                AllERUSList = await erus.To<ERUVM>().ToListAsync();

                foreach (var item in AllERUSList)
                {
                    item.ProfessionalTrainingName = keyValues.FirstOrDefault(k => item.IdProfessionalTraining == k.IdKeyValue)?.Name ?? string.Empty;
                    item.NKRLevelName = keyValues.FirstOrDefault(k => item.IdNKRLevel == k.IdKeyValue)?.Name ?? string.Empty;
                    item.EKRLevelName = keyValues.FirstOrDefault(k => item.IdEKRLevel == k.IdKeyValue)?.Name ?? string.Empty;
                }
            });
        }

        public async Task ReloadMenuNode()
        {
            await _locker.LockAsync(async () =>
            {
                AllMenuNodeList = new List<MenuNodeVM>();
                IQueryable<MenuNode> MenuNodes = this.repository.AllReadonly<MenuNode>();
                AllMenuNodeList = MenuNodes.To<MenuNodeVM>().ToList();
            });
        }


        public async Task ReloadPolicies()
        {
            await _locker.LockAsync(async () =>
            {
                AllPolicyList = new List<PolicyVM>();
                IQueryable<Policy> policies = this.repository.AllReadonly<Policy>();
                AllPolicyList = policies.To<PolicyVM>().ToList();
            });
        }

        public async Task<PolicyVM> GetPolicyByCode(string code)
        {
            PolicyVM policy = AllPolicyList.FirstOrDefault(s => s.PolicyCode == code);
            return policy;
        }

        public int GetActiveStatusID()
        {
            KeyType keyType = AllKeyTypeList.Where(x => x.KeyTypeIntCode == "StatusSPPOO").First();

            return AllKeyValueList.Where(x => x.IdKeyType == keyType.IdKeyType && x.KeyValueIntCode == "Active").First().IdKeyValue;
        }

        public int GetRemoveStatusID()
        {
            KeyType keyType = AllKeyTypeList.Where(x => x.KeyTypeIntCode == "StatusSPPOO").First();

            return AllKeyValueList.Where(x => x.IdKeyType == keyType.IdKeyType && x.KeyValueIntCode == "Inactive").First().IdKeyValue;
        }

        public int GetWorkStatusID()
        {
            KeyType keyType = AllKeyTypeList.Where(x => x.KeyTypeIntCode == "StatusSPPOO").First();

            return AllKeyValueList.Where(x => x.IdKeyType == keyType.IdKeyType && x.KeyValueIntCode == "Draft").First().IdKeyValue;
        }


        public int GetOrderAddTypechangeID()
        {
            KeyType keyType = AllKeyTypeList.Where(x => x.KeyTypeIntCode == "SPPOOOrderChange").First();

            return AllKeyValueList.Where(x => x.IdKeyType == keyType.IdKeyType && x.KeyValueIntCode == "Created").First().IdKeyValue;
        }

        public int GetOrderChangeTypechangeID()
        {
            KeyType keyType = AllKeyTypeList.Where(x => x.KeyTypeIntCode == "SPPOOOrderChange").First();

            return AllKeyValueList.Where(x => x.IdKeyType == keyType.IdKeyType && x.KeyValueIntCode == "Changed").First().IdKeyValue;
        }

        public int GetOrderRemoveTypechangeID()
        {
            KeyType keyType = AllKeyTypeList.Where(x => x.KeyTypeIntCode == "SPPOOOrderChange").First();

            return AllKeyValueList.Where(x => x.IdKeyType == keyType.IdKeyType && x.KeyValueIntCode == "Deleted").First().IdKeyValue;
        }
    }
}
