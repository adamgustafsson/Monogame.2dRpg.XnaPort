using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using LudosProduction.RpgLib;

namespace View
{
    /// <summary>
    /// Klass för hantering och utritning av dialogrutor och interaktions knappar
    /// </summary>
    class Conversation
    {
        #region Variabler

        public List<LudosProduction.RpgLib.Dialogue> m_dialogueList;
        private SpriteBatch m_spriteBatch;
        private SpriteFont m_spriteFont;
        private Rectangle m_textRect;
        private Rectangle m_speakerRect;
        private Camera m_camera;
        private bool m_drawDialog;
        private Texture2D m_dialogueWindow;

        private Model.QuestSystem m_questSystem;
        private View.InputHandler m_inputHandler;

        #endregion

        public Conversation(SpriteBatch a_spriteBatch, Model.GameModel a_gameModel, Camera a_camera, View.InputHandler a_inputHandler)
        {
            this.m_spriteBatch = a_spriteBatch;
            this.m_dialogueList = new List<Dialogue>();
            this.m_questSystem = a_gameModel.m_questSystem;
            this.m_camera = a_camera;
            this.m_inputHandler = a_inputHandler;
        }

        public void LoadContent(ContentManager a_content)
        {
            m_dialogueWindow = a_content.Load<Texture2D>("Textures/Conversation/bgprat");
            m_dialogueList = RpgXmlSerializer.LoadDialogsFromXml("Content/XML/dialogue.xml");
            m_spriteFont = a_content.Load<SpriteFont>(@"Fonts\Segoe");
        }

        //Metod för utritning av konversations-text
        public void DrawNPCText(Model.Unit a_friend, bool a_isQuestDialog)
        {
            //TextRektangel
            m_textRect = m_camera.VisualizeRectangle(new Rectangle(a_friend.ThisUnit.Bounds.X, a_friend.ThisUnit.Bounds.Y, 300, 50));

            //Visualiserar mapobjektets rektangel
            m_speakerRect = m_camera.VisualizeRectangle(new Rectangle(a_friend.ThisUnit.Bounds.X, a_friend.ThisUnit.Bounds.Y, a_friend.ThisUnit.Bounds.Width, a_friend.ThisUnit.Bounds.Height));

            //Hämtar meddelande
            string message = null;
            int state = 0;

            //Om det är en QuestDialog
            if (a_isQuestDialog)
                message = GetQuestMessage(m_textRect);
            //Annars en vanlig dialog    
            else
            {
                //Om spelet passerat q3 befinner sig dialogstaten i state 1, annars 0
                if (m_questSystem.CurrentQuest.Id > 2)
                {
                    state = 1;
                }
                message = GetMessage(a_friend.UnitId, state);
            }

            //Om textrektangeln överlappar med talaren flyttas den i yled
            if (m_textRect.Intersects(m_speakerRect))
            {
                int overlap = m_textRect.Bottom - m_speakerRect.Top;
                m_textRect.Y -= overlap;
            }

            //Ritar textruta + text
            if (m_drawDialog)
            {
                m_spriteBatch.Draw(m_dialogueWindow, m_textRect, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);
                m_spriteBatch.DrawString(m_spriteFont, message, new Vector2(m_textRect.X + 12, m_textRect.Y + 12), Color.White);

                if (a_isQuestDialog)
                    DrawQuestButtons();
            }
        }

        //Metod för utritning av interaktionsknappar
        private void DrawQuestButtons()
        {
            //Knappmått
            int buttonWidth = (int)(270f * 0.27f);
            int buttonHeight = (int)(100f * 0.26f); //Minskar storleken på en vanlig pratbubbla
            int textMargin = 7;
            Color colorOne = Color.White;
            Color colorTwo = Color.White;
            string buttonOneText;
            string buttonTwoText;

            //Knapprektanglar
            Rectangle buttonOne = new Rectangle(m_textRect.Right - (buttonWidth * 2), m_textRect.Bottom, buttonWidth, buttonHeight);
            Rectangle buttonTwo = new Rectangle(m_textRect.Right - buttonWidth, m_textRect.Bottom, buttonWidth, buttonHeight);

            if (m_questSystem.QuestStatus == Model.QuestSystem.PRE)
            {
                buttonOneText = "Accept";
                buttonTwoText = "Decline";
                m_spriteBatch.Draw(m_dialogueWindow, new Vector2(m_textRect.Right - buttonWidth, m_textRect.Bottom), new Rectangle(0, 0, 270, 100), Color.White, 0f, Vector2.Zero, 0.26f, SpriteEffects.None, 0f);
                m_spriteBatch.Draw(m_dialogueWindow, new Vector2(m_textRect.Right - (buttonWidth * 2), m_textRect.Bottom), new Rectangle(0, 0, 270, 100), Color.White, 0f, Vector2.Zero, 0.26f, SpriteEffects.None, 0f);

                if (m_inputHandler.MouseIsOver(buttonOne))
                {
                    colorOne = Color.Green;
                    if (m_inputHandler.DidGetTargetedByLeftClick(buttonOne))
                    {
                        m_questSystem.QuestStatus = Model.QuestSystem.MID;
                    }
                }

                if (m_inputHandler.MouseIsOver(buttonTwo))
                {
                    colorTwo = Color.Red;
                    if (m_inputHandler.DidGetTargetedByLeftClick(buttonTwo))
                    {
                        m_drawDialog = false;
                    }
                }

                m_spriteBatch.DrawString(m_spriteFont, buttonOneText, new Vector2(buttonOne.X + 7, buttonOne.Y + 4), colorOne);

            }
            else
            {
                if ((m_questSystem.QuestStatus == Model.QuestSystem.MID))
                {
                    buttonTwoText = "Okey";
                }
                else
                {
                    buttonTwoText = "Complete";
                    textMargin = 4;
                }

                if (m_inputHandler.MouseIsOver(buttonTwo))
                {
                    colorTwo = Color.Green;
                    if (m_inputHandler.DidGetTargetedByLeftClick(buttonTwo))
                    {
                        m_drawDialog = false;
                        if (buttonTwoText == "Complete")
                        {
                            m_questSystem.ActivateNextQuest();
                        }
                    }

                }

                m_spriteBatch.Draw(m_dialogueWindow, new Vector2(m_textRect.Right - buttonWidth, m_textRect.Bottom), new Rectangle(0, 0, 270, 100), Color.White, 0f, Vector2.Zero, 0.26f, SpriteEffects.None, 0f);
            }

            m_spriteBatch.DrawString(m_spriteFont, buttonTwoText, new Vector2(buttonTwo.X + textMargin, buttonTwo.Y + 4), colorTwo);
        }

        //Metod för anpassning av en textsträng enligt angivna rektangelmått
        public string ConstrainText(String message, Rectangle a_rectangle)
        {
            bool filled = false;
            string line = "";
            string returnString = "";
            string[] wordArray = message.Split(' ');


            // Går i genom varje ord i strängen
            foreach (string word in wordArray)
            {
                // Om nästa ord gör att vi överstiger bestämd bredd
                if (m_spriteFont.MeasureString(line + word).X > a_rectangle.Width - 20)
                {
                    // Om en ny rad inte överstiger rutans höjd
                    if (m_spriteFont.MeasureString(returnString + line + "\n").Y < a_rectangle.Height)
                    {
                        returnString += line + "\n";
                        line = "";
                        //Space under sista raden
                        a_rectangle.Height += 18;
                    }
                    // Om den nya rade gör att höjden överskrids
                    else if (!filled)
                    {
                        filled = true;
                        returnString += line;
                        line = "";
                    }
                }
                line += word + " ";
            }
            m_textRect = a_rectangle;
            return returnString + line;
        }

        //Metod för hämtning av dialogmeddelande från XML fil
        public string GetMessage(int a_id, int a_stateIndex)
        {
            string m_message = null;

            foreach (Dialogue dialogue in m_dialogueList)
            {
                if (dialogue.id == a_id)
                {
                    m_message = dialogue.message[a_stateIndex].msg;
                    if (dialogue.message[a_stateIndex].choices != null)
                    {
                        foreach (string choice in dialogue.message[a_stateIndex].choices)
                        {
                            m_message += choice;
                        }
                    }
                }
            }

            return ConstrainText(m_message, m_textRect);
        }

        //Metod för hämtning av questmeddelande från XML fil
        public string GetQuestMessage(Rectangle a_rectangle)
        {
            return ConstrainText(m_questSystem.CurrentMessage, a_rectangle);
        }

        //Metod för hämtning av QuestLogMeddelande från XML fil
        public string GetLogMessage(Rectangle a_rectangle)
        {
            return ConstrainText(m_questSystem.CurrentQuest.MidMessage, a_rectangle);
        }

        #region Get/Set
        public bool DrawDialog
        {
            get { return m_drawDialog; }
            set { m_drawDialog = value; }
        }
        public Rectangle TextRect
        {
            get { return m_textRect; }
            set { m_textRect = value; }
        } 
        #endregion
    }
}
