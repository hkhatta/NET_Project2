using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace BlackJackLibrary
{
    // Shoe Class: Implements the WCF service contract
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class Shoe : IShoe
    {
        // Private member variables
        //Dictionary of cards: Key - name, value - integer value for a card
        private static readonly Dictionary<string, uint> ranks = new Dictionary<string, uint>()
        {
            { "Cardback", 0 },
            { "Ace", 1 },
            { "Two", 2 },
            { "Three", 3},
            { "Four", 4 },
            { "Five", 5 },
            { "Six", 6 },
            { "Seven", 7 },
            { "Eight", 8 },
            { "Nine", 9 },
            { "Ten", 10 },
            { "Jack", 11 },
            { "Queen", 13 },
            { "King", 14 },
        };

        //Dictionary of Suits: Key - name, value - integer value to represent a card in Hexadecimal
        //The hexadecimal value when converted to ascii and concatatenated with the rank
        //gererates a visual image for the card (see method To String in the Card.cs)
        private static readonly Dictionary<string, uint> suits = new Dictionary<string, uint>()
        {
            { "Spades", 10 },
            { "Hearts", 11 },
            { "Diamonds", 12 },
            { "Clubs", 13 },
        };

        // List of Card objects
        private List<Card> cards = new List<Card>();
        // Index of card to track cards in the list
        private int cardsIndex = 0;
        // Default number of decks = 6
        private uint numDecks = 6;  
        //List of callbacks (1 per client)
        private HashSet<ICallback> callbacks= new HashSet<ICallback>();

        // Default constructor: populates the cards collection
        public Shoe()
        {
            Repopulate();
        }

        // Public Properties
        // A property to allow any client to read/initialize the number of decks
        public uint NumDecks
        {            
            get { return numDecks; }
            set
            {
                if (value != numDecks)
                {
                    numDecks = value;
                    Repopulate();
                }
            }
        }

        // A property to allow any client to know how many Card objects are available to be drawn
        public uint NumCards
        {
            get { return (uint)(cards.Count - cardsIndex); }
        }

        //Public Methods
        //Draw method: Returns a Card; the next available Card in the cards collection
        public Card Draw()
        {
            if (cardsIndex == cards.Count)
            {
                throw new InvalidOperationException("No card available in the shoe.");
            }
            Card card = cards[cardsIndex++];
            //UpdateAllClients(false);
            Console.WriteLine($"Shoe dealing {cards[cardsIndex]}");
            return card;
        }

        //Shuffle Method: Shuffle the sequence of cards in the cards collection randomly
        public void Shuffle()
        {
            Console.WriteLine($"Shoe shuffling");

            Random rng = new Random();
            cards = cards.OrderBy(card => rng.Next()).ToList();

            cardsIndex = 0;
            UpdateAllClients(true);
        }

        public void RegisterForCallbacks()
        {
            ICallback callback = OperationContext.Current.GetCallbackChannel<ICallback>();
            //The Add method in the HashSet only adds a element if it is a new element
            //No need to test if callbacks contains the callback
            callbacks.Add(callback);
        }

        public void UnregisterForCallbacks()
        {
            ICallback callback = OperationContext.Current.GetCallbackChannel<ICallback>();
            //The Remoce method in the HashSet only adds a element if it is a new element
            //No need to test if callbacks contains the callback
            callbacks.Remove(callback);
        }

        private void UpdateAllClients(bool emptyHand)
        {
            LibraryCallback info = new LibraryCallback(NumCards, numDecks, emptyHand);

            foreach (ICallback calback in callbacks)
            {
                calback.UpdateClient(info);
            }
        }



        // Helper methods

        //Repopulate Method: Clears the cards collection, populates it with new cards
        // and then randomizes the cards collection
        private void Repopulate()
        {
            Console.WriteLine($"Shoe repopulating with {numDecks} decks");

            // clears the current list
            cards.Clear();

            // Re-populate the list with new cards
            for (int d = 0; d < numDecks; d++)
            {
                // For each suit in SuitID...
                foreach (var s in suits)
                {
                    // For each rank in RankdID...
                    foreach (var r in ranks)
                    {
                        // Create and add a new Card 
                        cards.Add(new Card(s.Value, r.Value));
                    }
                }
            }

            // Shuffles the cards in the list
            Shuffle();
        }

    } // end class
}
