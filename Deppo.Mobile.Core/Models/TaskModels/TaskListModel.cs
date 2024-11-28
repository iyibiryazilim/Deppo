using Deppo.Sys.Service.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Core.Models.TaskModels;

//public enum Priority
//{
//	High = 0,
//	Normal = 1,
//	Low = 2,
//	Critic = 3
//}

//public enum TaskStatus
//{
//	NotStarted = 0,
//	InProgress = 1,
//	Waiting = 2,
//	Completed = 3,
//	Cancelled = 4,
//}

public class TaskListModel : INotifyPropertyChanged, IDisposable
{
	private Guid _oid;
	private ApplicationUser? _user;
	private DateTime? _createdOn;
	private DateTime? _startOn;
	private DateTime? _endOn;
	private string _subject = string.Empty;
	private string _description = string.Empty;
	private string _priority = string.Empty;
	private string _status = string.Empty;

    public TaskListModel()
    {
        
    }

	public Guid Oid
	{
		get => _oid;
		set
		{
			if (_oid == value) return;
			_oid = value;
			NotifyPropertyChanged();
		}
	}

	public string Subject
	{
		get => _subject;
		set
		{
			if (_subject == value) return;
			_subject = value;
			NotifyPropertyChanged();
		}
	}

	public string Description
	{
		get => _description;
		set
		{
			if (_description == value) return;
			_description = value;
			NotifyPropertyChanged();
		}
	}

	public DateTime? CreatedOn
	{
		get => _createdOn;
		set
		{
			if (_createdOn == value) return;
			_createdOn = value;
			NotifyPropertyChanged();
		}
	}

	public ApplicationUser? User
	{
		get => _user;
		set
		{
			if (_user == value) return;
			_user = value;
			NotifyPropertyChanged();
		}
	}

	public DateTime? StartOn
	{
		get => _startOn;
		set
		{
			if (_startOn == value) return;
			_startOn = value;
			NotifyPropertyChanged();
		}
	}

	public string Priority
	{
		get => _priority;
		set
		{
			if (_priority == value) return;
			_priority = value;
			NotifyPropertyChanged();
			NotifyPropertyChanged(nameof(PriorityText));
			NotifyPropertyChanged(nameof(PriorityColor));
		}
	}

	public string Status
	{
		get => _status;
		set
		{
			if (_status == value) return;
			_status = value;
			NotifyPropertyChanged();
			NotifyPropertyChanged(nameof(StatusText));
			NotifyPropertyChanged(nameof(StatusColor));
			NotifyPropertyChanged(nameof(StatusTextColor));
		}
	}



	public string PriorityText
	{
		get
		{
			switch (Priority)
			{
				case "High": // High
					return "Yüksek";
				case "Normal": // Normal
					return "Normal";
				case "Low": // Low
					return "Düşük";
				case "Critic": // Critic
					return "Kritik";
				default:
					return "-";
			}
		}
	}

	public string PriorityColor
	{
		get
		{
			switch (Priority)
			{
				case "High": // High
					return "#FFB302";
				case "Normal": // Normal
					return "#56F000";
				case "Low": // Low
					return "#55D0DB";
				case "Critic": // Critic
					return "#FF2A04";
				default:
					return "#FFF000";
			}
		}
	}


	public string StatusText
	{
		get
		{
			switch (Status)
			{
				case "NotStarted": // NotStarted
					return "Başlamadı";
				case "InProgress": // InProgress
					return "Devam ediyor";
				case "Waiting": // Waiting
					return "Bekliyor";
				case "Completed": // Completed
					return "Tamamlandı";
				case "Cancelled": // Cancelled
					return "İptal";
				default:  
					return "-";
			}
		}
	}

	public string StatusTextColor
	{
		get
		{
			switch (Status)
			{
				case "NotStarted": // NotStarted
					return "#FFFFFF";
				case "InProgress": // InProgress
					return "#141414";
				case "Waiting": // Waiting
					return "#FFFFFF";
				case "Completed": // Completed
					return "#FFFFFF";
				case "Cancelled": // Cancelled
					return "#FFFFFF";
				default:
					return "#141414";
			}
		}
	}

	public string StatusColor
	{
		get
		{
			switch(Status)
			{
				case "NotStarted": // NotStarted
					return "#A4ABB6";
				case "InProgress": // InProgress
					return "#E6BE0C";
				case "Waiting": // Waiting
					return "#2CCCFF";
				case "Completed": // Completed
					return "#008000";
				case "Cancelled": // Cancelled
					return "#FF2A04";
				default:
					return "#F7F7F7";
			}
		}
	}

	public event PropertyChangedEventHandler? PropertyChanged;

	protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}

	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (disposing)
		{
			PropertyChanged = null;
		}
	}
}
