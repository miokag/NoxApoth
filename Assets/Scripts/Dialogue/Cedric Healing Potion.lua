dialogue = {
    node1 = {
        character = "Cedric",
        text = {
            { line = "Between the endless meetings and dodging angry chickens at the last festival, I’m feeling... less than my mayoral best."},
            { line = "A potion, perhaps?" , orderdescription = "Return to Mayoral Best.", customerorder = "Healing Potion"},
        }
    },

    node2 = {
        character = "Cedric",
        text = {
            { line = "Do you have a potion that could make me feel ten years younger?"},
            { line = "Or at least get me through another town meeting without groaning audibly?" , orderdescription = "Make Cedric feel younger.", customerorder = "Healing Potion"},
        }
    },

    node3 = {
        character = "Cedric",
        text = {
            { line = "Running a town is all fun and games until someone throws a tomato at you."},
            { line = "I need a potion to bounce back, preferably before the next town hall." , orderdescription = "Potion to bounce back.", customerorder = "Healing Potion"},
        }
    },

    node4 = {
        character = "Cedric",
        text = {
            { line = "Between the complaints, the parades, and the festivals, I’ve worn myself thinner than last year’s budget."},
            { line = "Something restorative, if you have it?" , orderdescription = "Restore Cedric.", customerorder = "Healing Potion"},
        }
    }
}

function GetDialogueNode(node)
    print("Getting node: " .. node)
    return dialogue[node]
end