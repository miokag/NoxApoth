dialogue = {
    shop1 = {
        character = "Cedric",
        text = {
            { line = "Happy to see that you're all set-up!" },
            { line = "Let me help you out and be your first customer!" },
            { line = "Wait for me to go to the front counter first and then you can take my order." },
        }
    },

    shop2 = {
        character = "Cedric",
        text = {
            { line = "I'm not too familiar with potions so I'm just going to tell you about what I need."},
            { line = "My body has been feeling achy since I've been doing overtime in the office for the past few days." },
            { line = "I would like a potion that would relieve my body pain." },
            { line = "I'll stay here and wait, since I can't move around much." },
            { line = "But I'll be guiding you by using magic, you'll see in a bit." , orderdescription = "Potion to relieve body pain.", customerorder = "Health Potion"},
        }
    },

    backshop = {
        character = "Cedric",
        text = {
            { line = "Hello, hello? Can you hear me?" },
            { line = "Villagers of this place can use magic, mine is to be able to speak to people in their heads." },
            { line = "Now, where you are is the back shop." },
            { line = "On the right is the kitchen, and on the left is a door that leads you outside." },
            { line = "We've always been close to nature, so there are a lot of ingredients there that you can use for your potions." },
            { line = "I suggest you go outside for some ingredient gathering first, you can't really do anything in the kitchen without them, right?" }
        }
    },

    exploration1 = {
        character = "Cedric",
        text = {
            { line = "Such a gorgeous area, right?" },
            { line = "I don't know much about the specific uses of each ingredient, I am just a mayor after all..." },
            { line = "But I'm pretty sure your grandfather left you a notebook for you to use as reference."},
        }
    },

    explorationTOC = {
        character = "Cedric",
        text = {
            { line = "Oh, how convenient! There's a table of contents page to help in navigating!"},
            { line = "That makes it really easy to get information about the ingredient you need!"},
        }
    },

    explorationTOC2 = {
        character = "Cedric",
        text = {
            { line = "Let's look at an ingredient!"},
        }
    },

    explorationIngredientPage = {
        character = "Cedric",
        text = {
            { line = "This should help you know how to find ingredients!"},
            { line = "No pictures...? Well I guess that would make sense since he already knew them like the back of his hand."},
            { line = "There's at least a vague description of how they look like on the first part of the page."},
            { line = "Below that are some observations from your grandfather I think." },
            { line = "You should remember those once you start gathering ingredients." },
            { line = "Once you finish that you can go back to the ingredient table of contents page through the ingredient bookmark on the upper right corner." },
        }
    },

    explorationPotionPage = {
        character = "Cedric",
        text = {
            { line = "Huh...? No recipes either?"},
            { line = "Your grandfather always did want to specifically cater to each villagers needs." },
            { line = "If there are any effects that's listed down on each ingredient, then that might help." },
            { line = "Since it's for my body aches maybe look for something that would relieve or numb them?" },
            { line = "Sorry, I don't know what I'm talking about so I'll leave it up to you!" },
        }
    }
}

function GetDialogueNode(node)
    print("Getting node: " .. node)
    return dialogue[node]
end