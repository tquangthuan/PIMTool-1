﻿using PIMTool.Client.Dictionary;
using PIMTool.Client.Extension;
using PIMTool.Client.Presentation.Commands;
using PIMTool.Client.Validation;
using PIMTool.Client.WebApiClient.Services;
using PIMTool.Services.Resource;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows.Input;

namespace PIMTool.Client.Presentation.ViewModels
{
    class DetailProjectViewModel : BaseViewModel , IDataErrorInfo
    {
        private readonly IProjectWebApiClient _projectWebApiClient;
        private MainViewModel _mainViewModel;
        private int _projectNumber;
        private string _projectName;
        private string _customer;
        private int _group;
        private string _member;
        private EStatusType _selectedStatus;
        private DateTime _startDate = DateTime.Now;
        private DateTime? _finishDate;
        private DetailProjectValidator _projectValidator;
        private ICommand _saveOrUpdateCommand;
        private bool _isEditMode = false;
        private bool _projectNumberEnable = true;
        private string _buttonMode = "Create Project";

        #region Getter Setter
        public ProjectResource EditedProject { get; set; }

        public bool IsEditMode
        {
            get { return _isEditMode; }
            set
            {
                _isEditMode = value;
            }
        }

        public bool ProjectNumberEnable
        {
            get { return _projectNumberEnable; }
            set
            {
                _projectNumberEnable = value;
                OnPropertyChanged(nameof(ProjectNumberEnable));
            }
        }

        public string ButtonMode
        {
            get { return _buttonMode; }
            set
            {
                _buttonMode = value;
                OnPropertyChanged(nameof(ButtonMode));
            }
        }

        public int ProjectNumber
        {
            get { return _projectNumber; }
            set
            {
                _projectNumber = value;
                OnPropertyChanged(nameof(ProjectNumber));
            }
        }
        public string ProjectName
        {
            get { return _projectName; }
            set
            {
                _projectName = value;
                OnPropertyChanged(nameof(ProjectName));
            }
        }
        public string Customer
        {
            get { return _customer; }
            set
            {
                _customer = value;
                OnPropertyChanged(nameof(Customer));
            }
        }
        public string Member
        {
            get { return _member; }
            set
            {
                _member = value;
                OnPropertyChanged(nameof(Member));
            }
        }
        public int Group
        {
            get { return _group; }
            set
            {
                _group = value;
                OnPropertyChanged(nameof(Group));
            }
        }
        
        public EStatusType SelectedStatus
        {
            get { return _selectedStatus; }
            set
            {
                if (_selectedStatus != value)
                {
                    _selectedStatus = value;
                    OnPropertyChanged(nameof(SelectedStatus));
                }
            }
        }
        public DateTime StartDate
        {
            get { return _startDate; }
            set
            {
                _startDate = value;
                OnPropertyChanged(nameof(StartDate));
            }
        }
        public DateTime? FinishDate
        {
            get { return _finishDate; }
            set
            {
                _finishDate = value;
                OnPropertyChanged(nameof(FinishDate));
            }
        }
        #endregion 
        public ICommand SaveOrUpdateCommand
        {
            get
            {
                return _saveOrUpdateCommand ?? (_saveOrUpdateCommand = new CommandHandler(() => SaveOrUpdateProject(), () => CanUpdateProject()));
            }
        }

        public bool CanUpdateProject()
        {
            return Error.Equals(string.Empty);
        }

        #region Validation setting
        public string this[string columnName]
        {
            get
            {
                var firstError = _projectValidator.Validate(this).Errors.FirstOrDefault(lol => lol.PropertyName == columnName);
                if (firstError != null)
                    return _projectValidator != null ? firstError.ErrorMessage : "";
                return "";
            }
        }
        public string Error
        {
            get
            {
                if (_projectValidator != null)
                {
                    var results = _projectValidator.Validate(this);
                    if (results != null && results.Errors.Any())
                    {
                        var errors = string.Join(Environment.NewLine, results.Errors.Select(x => x.ErrorMessage).ToArray());
                        return errors;
                    }
                }
                return string.Empty;
            }
        }
        #endregion

        public DetailProjectViewModel(MainViewModel mainViewModel, bool isEditMode, ProjectResource project = null)
        {
            IsEditMode = isEditMode;
            _mainViewModel = mainViewModel;
            _projectValidator = new DetailProjectValidator(_mainViewModel.Projects, isEditMode);
            _projectWebApiClient = _mainViewModel.ProjectWebApiClient;
            EditedProject = project;
            if (IsEditMode)
            {
                ProjectNumberEnable = false;
                ButtonMode = "Update";
                LoadProject();
            }
        }

        //Update Project to the DB
        private void SaveOrUpdateProject()
        {
            var project = new SaveProjectResource();
            project.ProjectNumber = ProjectNumber;
            project.GroupId = Group + 1;
            project.Customer = Customer;
            project.Name = ProjectName;
            project.Member = Member.Trim(' ');
            project.Status = Enum.GetName(typeof(EStatusType), SelectedStatus);
            project.StartDate = StartDate;
            project.FinishDate = FinishDate;
            if (!IsEditMode)
                _projectWebApiClient.SaveProject(project);
            else
                _projectWebApiClient.UpdateProject(project);

            Thread.Sleep(1000);
            _mainViewModel.ResetAllProject();
            _mainViewModel.ProjectListSelected = true;
            _mainViewModel.SelectedViewModel = new ProjectListViewModel(_mainViewModel);
        }

        //Load info of selected project to the form
        private void LoadProject()
        {
            ProjectNumber = EditedProject.ProjectNumber;
            ProjectName = EditedProject.Name;
            Customer = EditedProject.Customer;
            Group = EditedProject.GroupId - 1;
            Member = EditedProject.Member;
            SelectedStatus = EnumHelper.GetEnumValue<EStatusType>(EditedProject.Status);
            StartDate = EditedProject.StartDate;
            FinishDate = EditedProject.FinishDate;
        }
    }
}