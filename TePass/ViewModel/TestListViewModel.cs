using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TePass.Models;
using TePass.Views;
using Xamarin.Forms;
using TePass.Services;

namespace TePass.ViewModels
{
    public class TestListViewModel : INotifyPropertyChanged
    {
        #region Initialization
        bool initialized = false;   // была ли начальная инициализация
        private bool isBusy;    // идет ли загрузка с сервера
        private bool isVerify; // верификация
        private bool isNull = false;
        TestsService testsService = new TestsService();
        QuestionsService questionsService = new QuestionsService();
        VarientsService varientsService = new VarientsService();
        LoginService loginService = new LoginService();
        public QuestionIn QuestionIn;
        public string textQuest { get; set; }
        public string VoidCode { get; set; }
        public string Name { get; set; }
        public ObservableCollection<Test> Tests { get; set; }
        public ObservableCollection<Question> Questions { get; set; }
        public List<Varient> Varients { get; set; }
        public ObservableCollection<Varient> VarientsNotAnswer { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand BackCommand { protected set; get; }
        public ICommand DoAnswer { protected set; get; }
        public ICommand Check { protected set; get; }

        Test selectedTest { get; set; }
        Question selectedQuestion { get; set; }
        Varient selectedVarient { get; set; }
        public INavigation Navigation { get; set; }
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

        public TestListViewModel()
        {
            Tests = new ObservableCollection<Test>();
            Questions = new ObservableCollection<Question>();
            Varients = new List<Varient>();
            VarientsNotAnswer = new ObservableCollection<Varient>();
            IsBusy = false;
            DoAnswer = new Command(DoAnswerMethod);
            Check = new Command(DoCheck);
        }
        #endregion
        #region SelectedObjects
        public async void DoCheck()
        {
            IsBusy = true;
            await GetFriends();
            await GetQuestions();
            await GetVarients();
            IsBusy = false;
            Later();
        }
        private void Later()
        {
            Navigation.PushModalAsync(new QuestionIn(SelectedTest, this));
        }
        public int preResult;
        public int result;
        public async void DoAnswerMethod()
        {
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
            if (i < Questions.Count-1)
            {
                i++;
                await GetVarients();
            }
            else 
            {
                result = (int)Math.Round(((double)preResult / Questions.Count * 10), 0);
                i = 0;
                await QuestionIn.DisplayAlert("Отметка", $"{Name}, вы получили " + result.ToString() + ", поздравляю!", "ОК");
                await SendEmailAsync();
                _ = Navigation.PopModalAsync();
            }
        }
        private async Task SendEmailAsync()
        {
            IsBusy = true;
            try
            {
                MailAddress from = new MailAddress("servicetecon@gmail.com", "TeDevelopment");
                MailAddress to = new MailAddress(await GetUserMail(selectedTest.UserId));
                MailMessage m = new MailMessage(from, to);
                m.Subject = $"Ваш тест ({selectedTest.Name}) прошёл пользователь (учащийся) {Name}! Отметка - {result}";
                m.Body = $"Здравствуйте, ваш тест {selectedTest.Name} прошёл пользователь (учащийся) {Name} с отметкой {result}";
                preResult = 0;
                result = 0;
                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                smtp.Credentials = new NetworkCredential("servicetecon@gmail.com", "servicesTeCon");
                smtp.EnableSsl = true;
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
            string login = "";
            User user = await loginService.Get(userid);
            return user.Email;
            IsBusy = false;
            return "";
        }
        public Test SelectedTest
        {
            get
            {
                return selectedTest;
            }
            set
            {
                if (selectedTest != null && selectedTest.Id == 0 && value != null)
                {
                    Test tempTest = new Test()
                    {
                        Id = value.Id,
                        Name = value.Name,
                        Questions = value.Questions,
                        Code = value.Code,
                        UserId = value.UserId
                    };
                    selectedTest = tempTest;
                    OnPropertyChanged("SelectedTest");
                }
                else if (selectedTest != null && value == null)
                {
                    selectedTest = null;
                    OnPropertyChanged("SelectedTest");
                }
                else if (selectedTest != value)
                {
                    Test tempTest = new Test()
                    {
                        Id = value.Id,
                        Name = value.Name,
                        Questions = value.Questions,
                        Code = value.Code,
                        UserId = value.UserId
                    };
                    selectedTest = tempTest;
                    OnPropertyChanged("SelectedTest");
                }
            }
        }
        public Question SelectedQuestion
        {
            get
            {
                return selectedQuestion;
            }
            set
            {
                if (selectedQuestion != null && selectedQuestion.Id == 0 && value != null)
                {
                    Question tempQuestion = new Question()
                    {
                        Id = value.Id,
                        OQuestion = value.OQuestion,
                        Varients = value.Varients,
                        TestId = value.Id
                    };
                    selectedQuestion = tempQuestion;
                    OnPropertyChanged("SelectedQuestion");
                }
                else if (selectedQuestion != null && value == null)
                {
                    selectedQuestion = null;
                    OnPropertyChanged("SelectedQuestion");
                }
                else if (selectedQuestion != value)
                {
                    Question tempQuestion = new Question()
                    {
                        Id = value.Id,
                        OQuestion = value.OQuestion,
                        Varients = value.Varients,
                        TestId = value.Id
                    };
                    selectedQuestion = tempQuestion;
                    OnPropertyChanged("SelectedQuestion");
                    //Navigation.PushModalAsync(new PageQuestConst(tempQuestion.TestId, tempQuestion, this));
                }
            }
        }
        public Varient SelectedVarient
        {
            get { return selectedVarient; }
            set
            {
                if (selectedVarient != value)
                {
                    Varient tempVarient = new Varient()
                    {
                        Id = value.Id,
                        IsTrue = value.IsTrue,
                        OVarient = value.OVarient,
                        QuestionId = value.QuestionId
                    };
                    selectedVarient = null;
                    OnPropertyChanged("SelectedVarient");
                    //Navigation.PushModalAsync(new PageVarient(tempVarient.QuestionId, tempVarient, this));
                }
            }
        }
        protected void OnPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
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
            IEnumerable<Question> questions = await questionsService.GetQuestByTestId(selectedTest.Id);
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
                IEnumerable<Varient> varients = await varientsService.GetVarientByQuestId(selectedQuestion.Id);

                // добавляем загруженные данные
                Varients.Clear();
                VarientsNotAnswer.Clear();
                while (Varients.Any())
                    Varients.RemoveAt(Varients.Count - 1);
                while (VarientsNotAnswer.Any())
                    VarientsNotAnswer.RemoveAt(VarientsNotAnswer.Count - 1);

                textQuest = Questions[i].OQuestion;
                OnPropertyChanged("textQuest");
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
 