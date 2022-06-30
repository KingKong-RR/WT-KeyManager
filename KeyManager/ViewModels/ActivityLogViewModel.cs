using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Windows;
using KeyManager.Commands.AsyncCommands;
using KeyManager.DataBase;
using KeyManager.Exceptions;
using KeyManager.Extensions;
using KeyManager.Models;
using KeyManager.Utilities;


namespace KeyManager.ViewModels
{
    public class ActivityLogViewModel : ViewModelBase
    {
        //Commands
        public IAsyncCommand DoGetActivityLogBySearchParameterAsync { get; }

        //Properties
        public ObservableCollection<ActivityLog> ActivityLog { get; } = new ObservableCollection<ActivityLog>();
        public string UserName { get; set; }
        public DateTime? SearchActivityLogStartDate { get; set; }
        public DateTime? SearchActivityLogEndDate { get; set; }

        public ActivityLogViewModel()
        {
            // Shows how to retrieve a customer by SearchParameter
            DoGetActivityLogBySearchParameterAsync = AsyncCommand.Create(async () =>
            {
                if (TextBoxFilter.ContainsSpecialChars(UserName))
                {
                    MessageBox.Show("Sonderzeichen sind nicht erlaubt.\nDas Feld Benutzer überprüfen", "Eingabefehler");
                    return;
                }

                try
                {

                    ObservableCollection<ActivityLog> resultActivityLog = await DataAccessService.GetActivityLogAsync(UserName ?? "", SearchActivityLogStartDate, SearchActivityLogEndDate);
                    ActivityLog.Clear();
                    ActivityLog.AddRange(resultActivityLog);
                }
                catch (UnauthorizedAccessException)
                {
                    MessageBox.Show("Sie sind nicht Autorisiert", "Autorisierungsfehler");
                }
                catch (NotFoundException)
                {
                    MessageBox.Show("Datensatz nicht gefunden, Eingabe überprüfen", "Suchfehler");
                }
                catch (HttpRequestException)
                {
                    // lost connection
                    OpenLogin.OpenLoginDialog();
                }
                catch (InternalServerErrorException)
                {
                    MessageBox.Show("Serverfehler, bitte erneut versuchen", "Serverfehler");
                }
                catch (Exception)
                {
                    MessageBox.Show("Unerwarteter Fehler, dem Admin melden.", "Fehler");
                }
            }, param =>
            {
                // only activate button if we are not logged in
                var execution = ((AsyncCommand<object>)DoGetActivityLogBySearchParameterAsync).Execution;
                if (execution != null)
                    return execution.IsCompleted;

                return true;
            });
        }
    }
}