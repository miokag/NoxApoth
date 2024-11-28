dialogue = {
    node1 = {
        character = "Talia",
        text = {
            { line = "You’d think years of forging would make me fireproof, but apparently not."},
            { line = "Got a potion that can make me less... crispy?" , orderdescription = "Make Talia less crispy.", customerorder = "Damage Resistant Potion"},
        }
    },

    node2 = {
        character = "Talia",
        text = {
            { line = "Sparks, splinters, and sharp edges—oh my!"},
            { line = "Got a potion to keep me from becoming the smithy’s latest casualty?" , orderdescription = "To avoid the latest casualty.", customerorder = "Damage Resistant Potion"},
        }
    },

    node3 = {
        character = "Talia",
        text = {
            { line = "Between the heat of the forge and the sharpness of my tools, I’m basically in a fight with my own shop."},
            { line = "I need a potion that’ll let me win for once!" , orderdescription = "Win against the Talia's shop.", customerorder = "Damage Resistant Potion"},
        }
    },

    node4 = {
        character = "Talia",
        text = {
            { line = "Let’s just say my forge and I have a complicated relationship."},
            { line = "I need something to make me less... breakable." , orderdescription = "Make Talia less breakable", customerorder = "Damage Resistant Potion"},
        }
    }
}

function GetDialogueNode(node)
    print("Getting node: " .. node)
    return dialogue[node]
end