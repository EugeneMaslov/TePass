using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using TePass.Models;
using TePass.Services;
using TePass.Views;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace TePass.ViewModels
{
    public class TestListViewModel : INotifyPropertyChanged
    {
        #region Initialization
        private bool initialized;   // была ли начальная инициализация
        private bool isBusy;    // идет ли загрузка с сервера
        private bool isVerify; // верификация
        private bool isNull;
        private bool isNotConnection;
        private bool isNoTest;
        private TestsService testsService = new TestsService();
        private QuestionsService questionsService = new QuestionsService();
        private VarientsService varientsService = new VarientsService();
        private LoginService loginService = new LoginService();
        protected string selectedLanguage = "English";

        public QuestionIn QuestionIn { get; set; }
        public string TextQuest { get; set; }
        public string VoidCode { get; set; }
        public string Name { get; set; }
        public ObservableCollection<Test> Tests { get; set; }
        public ObservableCollection<Question> Questions { get; set; }
        public List<Varient> Varients { get; set; }
        public ObservableCollection<Varient> VarientsNotAnswer { get; set; }
        public ObservableCollection<string> Languages { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand BackCommand { protected set; get; }
        public ICommand DoAnswer { protected set; get; }
        public ICommand Check { protected set; get; }
        public ICommand Lang { protected set; get; }

        public Test FSelectedTest { get; set; }
        public Question FSelectedQuestion { get; set; }
        public Varient FSelectedVarient { get; set; }
        public INavigation Navigation { get; set; }

        public string SelectedLanguage
        {
            get
            {
                return selectedLanguage;
            }
            set
            {
                if (selectedLanguage != null)
                {
                    selectedLanguage = value;
                    Navigation.PopModalAsync();
                }
                else selectedLanguage = "English";
                OnPropertyChanged("SelectedLanguage");
            }
        }

        public bool IsBusy
        {
            get { return isBusy; }
            set
            {
                isBusy = value;
                OnPropertyChanged("IsBusy");
                OnPropertyChanged("IsLoaded");
            }
        }
        public bool IsLoaded
        {
            get { return !isBusy; }
        }
        public bool IsVerify
        {
            get { return isVerify; }
            set
            {
                isVerify = value;
                OnPropertyChanged("IsVerify");
            }
        }

        public bool IsNoConnection
        {
            get { return isNotConnection; }
            set
            {
                if (isNotConnection != value)
                {
                    isNotConnection = value;
                    OnPropertyChanged("IsNoConnection");
                }
            }
        }

        public bool IsNoTest
        {
            get { return isNoTest; }
            set 
            {
                if (isNoTest != value)
                {
                    isNoTest = value;
                    OnPropertyChanged("IsNoTest");
                }
            }
        }
        
        public bool IsNull
        {
            get { return isNull; }
            set
            {
                if (isNull != value)
                {
                    isNull = value;
                    OnPropertyChanged("IsNull");
                }
            }
        }

        public bool Init
        {
            get { return initialized; }
            set
            {
                if (initialized != value)
                {
                    initialized = value;
                }
            }
        }

        public TestsService TestsService
        {
            get { return testsService; }
            set
            {
                if (testsService != value)
                {
                    testsService = value;
                }
            }
        }

        public QuestionsService QuestionsService
        {
            get { return questionsService; }
            set
            {
                if (questionsService != value)
                {
                    questionsService = value;
                }
            }
        }

        public VarientsService VarientsService
        {
            get { return varientsService; }
            set
            {
                if (varientsService != value)
                {
                    varientsService = value;
                }
            }
        }

        public LoginService LoginService
        {
            get { return loginService; }
            set
            {
                if (loginService != value)
                {
                    loginService = value;
                }
            }
        }

        public TestListViewModel()
        {
            Tests = new ObservableCollection<Test>();
            Questions = new ObservableCollection<Question>();
            Varients = new List<Varient>();
            VarientsNotAnswer = new ObservableCollection<Varient>();
            IsBusy = false;
            isNull = false;
            DoAnswer = new Command(DoAnswerMethod);
            Check = new Command(DoCheck);
            BackCommand = new Command(BackMethod);
            Lang = new Command(LangChange);
            Languages = new ObservableCollection<string>() { "English", "Русский", "Беларуская" };
        }

        public void BackMethod()
        {
            Navigation.PopModalAsync();
        }

        public void LangChange()
        {
            Navigation.PushModalAsync(new Language(this));
        }

        #endregion
        #region SelectedObjects
        public async void DoCheck()
        {
            IsBusy = true;
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                IsNoConnection = false;
                await GetFriends();
                if (FSelectedTest != null)
                {
                    IsNoTest = false;
                    await GetQuestions();
                    await GetVarients();
                    IsBusy = false;
                    Later();
                }
                else
                {
                    IsNoTest = true;
                    IsBusy = false;
                }
            }
            else
            {
                IsNoConnection = true;
                IsBusy = false;
            }
        }
        private void Later()
        {
            Navigation.PushModalAsync(new QuestionIn(SelectedTest, this));
        }
        public int preResult;
        public int result;
        public async void DoAnswerMethod()
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                try
                {
                    IsNoConnection = false;
                    bool preTrue = false;
                    for (int i = 0; i < Varients.Count; i++)
                    {
                        if (Varients[i].IsTrue == VarientsNotAnswer[i].IsTrue)
                        {
                            preTrue = true;
                        }
                        else
                        {
                            preTrue = false;
                            break;
                        }
                    }
                    if (preTrue)
                    {
                        preResult++;
                    }
                    if (i < Questions.Count - 1)
                    {
                        i++;
                        await GetVarients();
                    }
                    else
                    {
                        result = (int)Math.Round((double)preResult / Questions.Count * 10, 0);
                        i = 0;
                        await QuestionIn.DisplayAlert("Отметка", $"{Name}, вы получили " + result.ToString() + ", поздравляю!", "ОК");
                        await SendEmailAsync();
                        await Navigation.PopModalAsync();
                    }
                }
                catch (Exception)
                {
                    await Navigation.PopModalAsync();
                }
            }
            else
            {
                IsNoConnection = true;
                isBusy = true;
                Thread.Sleep(1000);
                DoAnswerMethod();
            }
        }
        private async Task SendEmailAsync()
        {
            IsBusy = true;
            try
            {
                MailAddress from = new MailAddress("servicetecon@gmail.com", "TeDevelopment");
                MailAddress to = new MailAddress(await GetUserMail(FSelectedTest.UserId));
                MailMessage m = new MailMessage(from, to)
                {
                    Subject = $"Ваш тест ({FSelectedTest.Name}) прошёл пользователь (учащийся) {Name}! Отметка - {result}",
                    Body = $"Здравствуйте, ваш тест {FSelectedTest.Name} прошёл пользователь (учащийся) {Name} с отметкой {result}"
                };
                preResult = 0;
                result = 0;
                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new NetworkCredential("servicetecon@gmail.com", "servicesTeCon"),
                    EnableSsl = true
                };
                await smtp.SendMailAsync(m);
            }
            catch (Exception)
            {
                preResult = 0;
                result = 0;
            }
            IsBusy = false;
        }
        public async Task<string> GetUserMail(int userid)
        {
            IsBusy = true;
            User user = await loginService.Get(userid);
            IsBusy = false;
            return user.Email;
        }
        public Test SelectedTest
        {
            get
            {
                return FSelectedTest;
            }
            set
            {
                if (FSelectedTest != null && FSelectedTest.Id == 0 && value != null)
                {
                    Test tempTest = new Test()
                    {
                        Id = value.Id,
                        Name = value.Name,
                        Questions = value.Questions,
                        Code = value.Code,
                        UserId = value.UserId
                    };
                    FSelectedTest = tempTest;
                    OnPropertyChanged("SelectedTest");
                }
                else if (FSelectedTest != null && value == null)
                {
                    FSelectedTest = null;
                    OnPropertyChanged("SelectedTest");
                }
                else if (FSelectedTest != value)
                {
                    Test tempTest = new Test()
                    {
                        Id = value.Id,
                        Name = value.Name,
                        Questions = value.Questions,
                        Code = value.Code,
                        UserId = value.UserId
                    };
                    FSelectedTest = tempTest;
                    OnPropertyChanged("SelectedTest");
                }
            }
        }
        public Question SelectedQuestion
        {
            get
            {
                return FSelectedQuestion;
            }
            set
            {
                if (FSelectedQuestion != null && FSelectedQuestion.Id == 0 && value != null)
                {
                    Question tempQuestion = new Question()
                    {
                        Id = value.Id,
                        OQuestion = value.OQuestion,
                        Varients = value.Varients,
                        TestId = value.Id
                    };
                    FSelectedQuestion = tempQuestion;
                    OnPropertyChanged("SelectedQuestion");
                }
                else if (FSelectedQuestion != null && value == null)
                {
                    FSelectedQuestion = null;
                    OnPropertyChanged("SelectedQuestion");
                }
                else if (FSelectedQuestion != value)
                {
                    Question tempQuestion = new Question()
                    {
                        Id = value.Id,
                        OQuestion = value.OQuestion,
                        Varients = value.Varients,
                        TestId = value.Id
                    };
                    FSelectedQuestion = tempQuestion;
                    OnPropertyChanged("SelectedQuestion");
                }
            }
        }
        public Varient SelectedVarient
        {
            get { return FSelectedVarient; }
            set
            {
                if (FSelectedVarient != value)
                {
                    FSelectedVarient = null;
                    OnPropertyChanged("SelectedVarient");
                }
            }
        }
        protected void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
        #endregion
        #region Getting on Server
        public async Task GetFriends()
        {
            if (initialized == true) return;
            IsBusy = true;
            Test test = await testsService.GetTestByCode(VoidCode);

            // очищаем список
            Tests.Clear();
            while (Tests.Any())
                Tests.RemoveAt(Tests.Count - 1);

            // добавляем загруженные данные
            Tests.Add(test);
            SelectedTest = test;

            IsBusy = false;
        }
        public async Task GetQuestions()
        {
            if (initialized == true) return;
            IsBusy = true;
            IEnumerable<Question> questions = await questionsService.GetQuestByTestId(FSelectedTest.Id);
            // очищаем список
            Questions.Clear();
            while (Questions.Any())
                Questions.RemoveAt(Questions.Count - 1);

            // добавляем загруженные данные
            foreach (Question f in questions)
            {
                if (!Questions.Contains(f))
                {
                    Questions.Add(f);
                }
            }
        }
        public int i = 0;
        public async Task GetVarients()
        {
            if (initialized == true) return;
            IsBusy = true;
            if (Questions.Count > 0)
            {
                SelectedQuestion = Questions[i];
                IEnumerable<Varient> varients = await varientsService.GetVarientByQuestId(FSelectedQuestion.Id);

                // добавляем загруженные данные
                Varients.Clear();
                VarientsNotAnswer.Clear();
                while (Varients.Any())
                    Varients.RemoveAt(Varients.Count - 1);
                while (VarientsNotAnswer.Any())
                    VarientsNotAnswer.RemoveAt(VarientsNotAnswer.Count - 1);

                TextQuest = Questions[i].OQuestion;
                OnPropertyChanged("TextQuest");
                foreach (Varient f in varients)
                {
                    if (!Varients.Contains(f))
                    {
                        Varients.Add(f);
                    }
                }
                foreach (Varient item in Varients)
                {
                    VarientsNotAnswer.Add((Varient)item.CloneNotTrue());
                }
                OnPropertyChanged("VarientsNotAnswer");
            }
            IsBusy = false;
        }
        #endregion
    }
}
 