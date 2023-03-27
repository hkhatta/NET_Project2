/*
 * Program:         CardsGuiClient.exe (.NET Framework version)
 * Module:          MainWindow.xaml.cs
 * Author:          T. Haworth
 * Date:            March 9, 2023
 * Description:     An Windows WPF client to use and demonstrate the features/services 
 *                  of the CardsLibrary.dll assembly. This version has been modified to 
 *                  use the Shoe class as a WCF service. It also uses an endpoint
 *                  configuration that is declared "administratively" in the client's
 *                  App.config file.
 *                  
 *                  Note that we had to add a reference to the .NET Framework 
 *                  assembly System.ServiceModel.dll.
 */

using System;
using System.Windows;
using System.ServiceModel;  // WCF types
using BlackJackLibrary;

namespace CardsGUIClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, ICallback
    {
        // --------------------- Member variables ---------------------
        private readonly IShoe shoe = null; // Note: Type IShoe instead of Shoe
        private bool isClientTurn = true;
        // ------------------------ Constructor -----------------------
        public MainWindow()
        {
            InitializeComponent();

            try
            {

                DuplexChannelFactory<IShoe> channel = new DuplexChannelFactory<IShoe>(this,"ShoeEndPoint");
                shoe = channel.CreateChannel();
                shoe.RegisterForCallbacks();
                Card card1 = shoe?.Draw();
                Card card2 = shoe?.Draw();
                ListCards.Items.Insert(0, card2);
                ListCards.Items.Insert(0, card1);

                //Initialize the GUI
                UpdateCardCounts();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        // ---------------------- Event handlers ----------------------

        // Runs when the user clicks the Hit button
        private void ButtonHit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (isClientTurn) // Check if it is the client's turn
                {
                    // Modified to receive a string instead of a Card object from Draw()
                    Card card = shoe?.Draw();
                    ListCards.Items.Insert(0, card);

                    int totalValue = 0;
                    foreach (Card c in ListCards.Items)
                    {
                        totalValue += (int)c.Rank;
                    }

                    if (totalValue > 21)
                    {
                        isClientTurn = false;
                    }

                    UpdateCardCounts();

                    if (!isClientTurn)
                    {
                        shoe?.Shuffle();
                        UpdateCardCounts();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // Runs when the user clicks the Stand button
        private void ButtonStand_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (isClientTurn) 
                {
                    isClientTurn = false; 
                }
                else
                {
                    shoe?.Shuffle();
                    UpdateCardCounts();

                    isClientTurn = true; 
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // Runs when the user clicks the Close button
        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // Runs when the user slides the slider control to change the number of decks

        // ---------------------- Helper methods ----------------------

        // Reinitializes the Shoe and Hand card counts in the GUI
        private void UpdateCardCounts()
        {
            
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            shoe?.UnregisterForCallbacks();
            (shoe as IClientChannel)?.Close();
        }

        private void ListCards_Copy_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }

        private void ListCards_Copy1_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }

        //ICallback interface method implementation
        //Receives an LibraryCallback object with the information from the library
        //Updates the client object with the information received
        public void UpdateClient(LibraryCallback info)
        {
            if (System.Threading.Thread.CurrentThread == this.Dispatcher.Thread)
            {
                //TO DO: Implement what needs to be updated
                if(info.IsEmptyHand)
                {
                    ListCards.Items.Clear();
                }
            }
            else
            {
                Action<LibraryCallback> updateDelegate = UpdateClient;
                this.Dispatcher.BeginInvoke(updateDelegate, info);
            }
        }
    } // end partial class
}
