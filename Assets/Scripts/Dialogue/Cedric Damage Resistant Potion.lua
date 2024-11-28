dialogue = {
    node1 = {
        character = "Cedric",
        text = {
            { line = "Do you have something to make me impervious to... let’s call it 'constructive feedback'?"},
            { line = "Preferably the sharp, pointy kind." , orderdescription = "Defend Cedric from constructive feedback.", customerorder = "Damage Resistant Potion"},
        }
    },

    node2 = {
        character = "Cedric",
        text = {
            { line = "Look, I’ve taken enough metaphorical hits from the council, so the literal ones are starting to wear thin."},
            { line = "Potion me, please!" , orderdescription = "For taking hits.", customerorder = "Damage Resistant Potion"},
        }
    },

    node3 = {
        character = "Cedric",
        text = {
            { line = "Some days, being mayor feels like a battlefield."},
            { line = "Got a potion that’ll make me feel more like a knight and less like a pincushion?" , orderdescription = "Make Cedric not a pincushion.", customerorder = "Damage Resistant Potion"},
        }
    },

    node4 = {
        character = "Cedric",
        text = {
            { line = "A mayor’s life is rough—dodging accusations, negotiating peace, and occasionally getting hit by a flying loaf of bread."},
            { line = "Help me endure." , orderdescription = "To combat the flying loaf of bread.", customerorder = "Damage Resistant Potion"},
        }
    }
}

function GetDialogueNode(node)
    print("Getting node: " .. node)
    return dialogue[node]
end