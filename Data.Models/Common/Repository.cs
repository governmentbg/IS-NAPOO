using Data.Models.Data.Archive;
using Data.Models.Data.Common;
 
using Data.Models.Data.Framework;
 
using Data.Models.DB;
using Data.Models.Framework;
using Data.Models;
using ISNAPOO.Common.Constants;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Security.Principal;

namespace Data.Models.Common
{
    /// <summary>
    /// Implementation of repository access methods
    /// for Relational Database Engine
    /// </summary>
    /// <typeparam name="T">Type of the data table to which 
    /// current reposity is attached</typeparam>
    public class Repository : IRepository
    {
        protected AuthenticationStateProvider authenticationStateProvider;
        private List<EventLog> eventLogList = new List<EventLog>();
        private List<IModifiable> modifiables;
        private IModifiable changeEntry;
        private UserProps userProps;
        public Repository(ApplicationDbContext context, AuthenticationStateProvider authenticationStateProvider)
        {
            this.Context = context;
            this.authenticationStateProvider = authenticationStateProvider;
            //this.userProps = new UserProps();
            /*
            try
            {
                AuthenticationState AuthenticationState = this.authenticationStateProvider.GetAuthenticationStateAsync().Result;

                if (!string.IsNullOrEmpty(AuthenticationState.User.FindFirstValue(GlobalConstants.ID_CANDIDATE_PROVIDER)))
                {
                    userProps.IdCandidateProvider = Int32.Parse(AuthenticationState.User.FindFirstValue(GlobalConstants.ID_CANDIDATE_PROVIDER));
                }

                if (!string.IsNullOrEmpty(AuthenticationState.User.FindFirstValue(ClaimTypes.NameIdentifier)))
                {
                    userProps.ID = AuthenticationState.User.FindFirstValue(ClaimTypes.NameIdentifier);
                }

                if (!string.IsNullOrEmpty(AuthenticationState.User.FindFirstValue(GlobalConstants.ID_USER)))
                {
                    userProps.UserId = Int32.Parse(AuthenticationState.User.FindFirstValue(GlobalConstants.ID_USER));
                }

                if (!string.IsNullOrEmpty(AuthenticationState.User.Identity.Name))
                {
                    userProps.UserName = AuthenticationState.User.Identity.Name;
                }

                if (!string.IsNullOrEmpty(AuthenticationState.User.FindFirstValue(GlobalConstants.PERSON_FULLNAME)))
                {
                    userProps.PersonName = AuthenticationState.User.FindFirstValue(GlobalConstants.PERSON_FULLNAME);
                }

                //AuthenticationState.User.Claims.Where(x=>x.Type == "remoteIpAddress").Select(s=>s.Value)
                if (AuthenticationState != null && AuthenticationState.User != null && AuthenticationState.User.Claims.Any(x => x.Type == "remoteIpAddress"))
                {
                    userProps.IPAddress = AuthenticationState.User.Claims.First(x => x.Type == "remoteIpAddress").Value;
                }
                if (AuthenticationState != null && AuthenticationState.User != null && AuthenticationState.User.Claims.Any(x => x.Type == "browserInformation"))
                {
                    userProps.BrowserInformation = AuthenticationState.User.Claims.First(x => x.Type == "browserInformation").Value;
                }


                if (AuthenticationState != null && AuthenticationState.User != null && AuthenticationState.User.Claims.Any(x => x.Type == "currentAction"))
                {
                    userProps.CurrentAction = AuthenticationState.User.Claims.First(x => x.Type == "currentAction").Value;
                }

                if (AuthenticationState != null && AuthenticationState.User != null && AuthenticationState.User.Claims.Any(x => x.Type == "currentActionDescription"))
                {
                    userProps.CurrentActionDescription = AuthenticationState.User.Claims.First(x => x.Type == "currentActionDescription").Value;
                }
            }
            catch { }

            */
        }


        private void LoadAuthenticationState() 
        {
            if (this.userProps == null)
            {
                this.userProps = new UserProps();

                try
                {
                    AuthenticationState AuthenticationState = this.authenticationStateProvider.GetAuthenticationStateAsync().Result;

                    if (!string.IsNullOrEmpty(AuthenticationState.User.FindFirstValue(GlobalConstants.ID_CANDIDATE_PROVIDER)))
                    {
                        userProps.IdCandidateProvider = Int32.Parse(AuthenticationState.User.FindFirstValue(GlobalConstants.ID_CANDIDATE_PROVIDER));
                    }

                    if (!string.IsNullOrEmpty(AuthenticationState.User.FindFirstValue(ClaimTypes.NameIdentifier)))
                    {
                        userProps.ID = AuthenticationState.User.FindFirstValue(ClaimTypes.NameIdentifier);
                    }

                    if (!string.IsNullOrEmpty(AuthenticationState.User.FindFirstValue(GlobalConstants.ID_USER)))
                    {
                        userProps.UserId = Int32.Parse(AuthenticationState.User.FindFirstValue(GlobalConstants.ID_USER));
                    }

                    if (!string.IsNullOrEmpty(AuthenticationState.User.Identity.Name))
                    {
                        userProps.UserName = AuthenticationState.User.Identity.Name;
                    }

                    if (!string.IsNullOrEmpty(AuthenticationState.User.FindFirstValue(GlobalConstants.PERSON_FULLNAME)))
                    {
                        userProps.PersonName = AuthenticationState.User.FindFirstValue(GlobalConstants.PERSON_FULLNAME);
                    }

                    //AuthenticationState.User.Claims.Where(x=>x.Type == "remoteIpAddress").Select(s=>s.Value)
                    if (AuthenticationState != null && AuthenticationState.User != null && AuthenticationState.User.Claims.Any(x => x.Type == "remoteIpAddress"))
                    {
                        userProps.IPAddress = AuthenticationState.User.Claims.First(x => x.Type == "remoteIpAddress").Value;
                    }
                    if (AuthenticationState != null && AuthenticationState.User != null && AuthenticationState.User.Claims.Any(x => x.Type == "browserInformation"))
                    {
                        userProps.BrowserInformation = AuthenticationState.User.Claims.First(x => x.Type == "browserInformation").Value;
                    }


                    if (AuthenticationState != null && AuthenticationState.User != null && AuthenticationState.User.Claims.Any(x => x.Type == "currentAction"))
                    {
                        userProps.CurrentAction = AuthenticationState.User.Claims.First(x => x.Type == "currentAction").Value;
                    }

                    if (AuthenticationState != null && AuthenticationState.User != null && AuthenticationState.User.Claims.Any(x => x.Type == "currentActionDescription"))
                    {
                        userProps.CurrentActionDescription = AuthenticationState.User.Claims.First(x => x.Type == "currentActionDescription").Value;
                    }

                    if (AuthenticationState != null && AuthenticationState.User != null && AuthenticationState.User.Claims.Any(x => x.Type == "currentUrl"))
                    {
                        userProps.CurrentUrl = AuthenticationState.User.Claims.First(x => x.Type == "currentUrl").Value;
                    }

                    if (AuthenticationState != null && AuthenticationState.User != null && AuthenticationState.User.Claims.Any(x => x.Type == "currentMenu"))
                    {
                        userProps.CurrentMenu = AuthenticationState.User.Claims.First(x => x.Type == "currentMenu").Value;
                    }
                }
                catch { }
            }
        }
         

        /// <summary>
        /// Entity framework DB context holding connection information and properties
        /// and tracking entity states 
        /// </summary>
        protected DbContext Context { get; set; }

        /// <summary>
        /// Representation of table in database
        /// </summary>
        protected DbSet<T> DbSet<T>() where T : class
        {
            return this.Context.Set<T>();
        }

        public IEnumerable<T> ExecuteProc<T>(string procedureName, params object[] args) where T : class
        {
            return this.Context.Set<T>().FromSqlRaw("/*NO LOAD BALANCE*/ select * from " + procedureName, args).ToList();
        }

        public IEnumerable<T> ExecuteSQL<T>(string query, params object[] args) where T : class
        {
            return this.Context.Set<T>().FromSqlRaw("/*NO LOAD BALANCE*/ " + query, args).ToList();
        }

        /// <summary>
        /// Adds entity to the database
        /// </summary>
        /// <param name="entity">Entity to add</param>
        public async Task AddAsync<T>(T entity) where T : class
        {
            await DbSet<T>().AddAsync(entity);
        }

        /// <summary>
        /// Ads collection of entities to the database
        /// </summary>
        /// <param name="entities">Enumerable list of entities</param>
        public async Task AddRangeAsync<T>(IEnumerable<T> entities) where T : class
        {
            await DbSet<T>().AddRangeAsync(entities);
        }

        /// <summary>
        /// All records in a table
        /// </summary>
        /// <returns>Queryable expression tree</returns>
        public IQueryable<T> All<T>() where T : class
        {
            return DbSet<T>().AsQueryable();
        }

        public IQueryable<T> All<T>(Expression<Func<T, bool>> search) where T : class
        {
            return this.DbSet<T>().Where(search).AsQueryable();
        }

        /// <summary>
        /// The result collection won't be tracked by the context
        /// </summary>
        /// <returns>Expression tree</returns>
        public IQueryable<T> AllReadonly<T>() where T : class
        {
            return this.DbSet<T>()
                .AsQueryable()
                .AsNoTracking();
        }
        public IQueryable<T> AllReadonly<T>(Expression<Func<T, bool>> search) where T : class
        {
            return this.DbSet<T>()
                .Where(search)
                .AsQueryable()
                .AsNoTracking();
        }

        /// <summary>
        /// Deletes a record from database
        /// </summary>
        /// <param name="id">Identificator of record to be deleted</param>
        public async Task HardDeleteAsync<T>(object id) where T : class
        {
            T entity = await GetByIdAsync<T>(id);

            HardDelete<T>(entity);
        }

        /// <summary>
        /// Deletes a record from database
        /// </summary>
        /// <param name="entity">Entity representing record to be deleted</param>
        public void HardDelete<T>(T entity) where T : class
        {
            EntityEntry entry = this.Context.Entry(entity);

            if (entry.State == EntityState.Detached)
            {
                this.DbSet<T>().Attach(entity);
            }

            entry.State = EntityState.Deleted;
        }

        /// <summary>
        /// Detaches given entity from the context
        /// </summary>
        /// <param name="entity">Entity to be detached</param>
        public void Detach<T>(T entity) where T : class
        {
            EntityEntry entry = this.Context.Entry(entity);

            entry.State = EntityState.Detached;
        }

        /// <summary>
        /// Disposing the context when it is not neede
        /// Don't have to call this method explicitely
        /// Leave it to the IoC container
        /// </summary>
        public void Dispose()
        {
            if (this.Context != null)
                this.Context.Dispose();
        }

        /// <summary>
        /// Gets specific record from database by primary key
        /// </summary>
        /// <param name="id">record identificator</param>
        /// <returns>Single record</returns>
        public async Task<T> GetByIdAsync<T>(object id) where T : class
        {
            var dbEntity = await DbSet<T>().FindAsync(id);

            DetachEntities();

            return dbEntity;
        }

        public async Task<T> GetByIdsAsync<T>(object[] id) where T : class
        {
            return await DbSet<T>().FindAsync(id);
        }

        public async Task<T> GetLastEntryAsync<T>() where T : class
        {
            return await DbSet<T>().OrderBy(x => x).LastOrDefaultAsync();
        }

        /// <summary>
        /// Saves all made changes in trasaction
        /// </summary>
        /// <returns>Error code</returns>
        public int SaveChanges(bool invokeUpdateModifiableEntities = true)
        {
            LoadAuthenticationState();
            try
            {
                UpdateModifiableEntities(invokeUpdateModifiableEntities);

                return this.Context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }



        public async Task<int> SaveChangesAsync(bool invokeUpdateModifiableEntities = true)
        {
            LoadAuthenticationState();
            try
            {
       
                UpdateModifiableEntities(invokeUpdateModifiableEntities);
                
                var count = await this.Context.SaveChangesAsync();
				
				// var events = await this.Context.SaveChangesAsync();
				DetachEntities();

                await InsertInEventLogTable();

                return count;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            }
        private async Task InsertInEventLogTable()
        {
 

			if (eventLogList.Any())
            {

                foreach (var log in eventLogList.Where(x => x.EventAction == "Added")) 
                {
                    log.EntityID = (log.AuditEntry.Entity as IEntity).IdEntity.ToString();
                }
                this.AddRangeAsync(eventLogList);  
				var events = await this.Context.SaveChangesAsync(); 
			}
        }

        private void UpdateModifiableEntities(bool invokeUpdateModifiableEntities)
        {
            eventLogList.Clear();

			var changedEntries = this.Context.ChangeTracker
                .Entries()
                .Where(x => x.Entity is IModifiable &&  (x.State == EntityState.Added || x.State == EntityState.Modified || x.State == EntityState.Deleted))                
                .ToList();


            foreach (var entry in changedEntries)
            {
                if (invokeUpdateModifiableEntities)
                {
                    var entity = (IModifiable)entry.Entity;

                    if (entry.State == EntityState.Added)
                    {
                        entity.IdCreateUser = userProps.UserId;
                        entity.CreationDate = DateTime.Now;
                        entity.IdModifyUser = userProps.UserId;
                        entity.ModifyDate = DateTime.Now;
                    }
                    else
                    {
                        entity.IdCreateUser = entity.IdCreateUser;
                        entity.CreationDate = entity.CreationDate;
                        entity.IdModifyUser = userProps.UserId;
                        entity.ModifyDate = DateTime.Now;
                    }
                }
                
              
                IEntity savedEntity = entry.Entity as IEntity;

            

                if (savedEntity != null)
                {
                    var eventLog = new EventLog();

                    var tableName = ((System.ComponentModel.DataAnnotations.Schema.TableAttribute)(entry.Entity).GetType().GetCustomAttribute(typeof(System.ComponentModel.DataAnnotations.Schema.TableAttribute))).Name;

                    eventLog.idUser = userProps.UserId;
                    eventLog.idAspNetUsers = userProps.ID;
                    eventLog.EventDate = DateTime.Now;
                    eventLog.EventMessage = $"Policy:{userProps.CurrentAction}, Description:{userProps.CurrentActionDescription}";
                    eventLog.EventAction = entry.State.ToString();
                    eventLog.EntityID = savedEntity.IdEntity.ToString();
                    eventLog.EntityName = tableName;
                    eventLog.PersonName = userProps.PersonName;
                    eventLog.IP = userProps.IPAddress;
                    eventLog.BrowserInformation = userProps.BrowserInformation;
                    eventLog.CurrentUrl = userProps.CurrentUrl;
                    eventLog.CurrentMenu = userProps.CurrentMenu;

                    eventLog.AuditEntry = entry;

                    eventLogList.Add(eventLog);
                }
               
            }            
        }

        private void DetachEntities()
        {
            var changedEntries = this.Context.ChangeTracker
                .Entries()
                .ToList();

            foreach (var entry in changedEntries)
            {
                entry.State = EntityState.Detached;
            }

        }

        /// <summary>
        /// Updates a record in database
        /// </summary>
        /// <param name="entity">Entity for record to be updated</param>
        public void Update<T>(T entity) where T : class
        {
            this.DbSet<T>().Update(entity);
        }

        /// <summary>
        /// Updates set of records in the database
        /// </summary>
        /// <param name="entities">Enumerable collection of entities to be updated</param>
        public void UpdateRange<T>(IEnumerable<T> entities) where T : class
        {
            this.DbSet<T>().UpdateRange(entities);
        }

        public void HardDeleteRange<T>(IEnumerable<T> entities) where T : class
        {
            this.DbSet<T>().RemoveRange(entities);
        }

        public void HardDeleteRange<T>(Expression<Func<T, bool>> deleteWhereClause) where T : class
        {
            var entities = All<T>(deleteWhereClause);
            HardDeleteRange(entities);
        }
    }
}
