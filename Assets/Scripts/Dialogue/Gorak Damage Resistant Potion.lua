dialogue = {
    node1 = {
        character = "Gorak",
        text = {
            { line = "If I get hit one more time, I might just ghost out for good!"},
            { line = "Got anything to keep me from getting flattened like a pancake?" , orderdescription = "Make Gorak to not be flattened.", customerorder = "Damage Resistant Potion"},
        }
    },

    node2 = {
        character = "Gorak",
        text = {
            { line = "I’d like to not fall apart at the slightest provocation, thanks."},
            { line = "Do you have anything that’ll keep me from being so... hittable?" , orderdescription = "Make Gorak to not be hittable", customerorder = "Damage Resistant Potion"},
        }
    },

    node3 = {
        character = "Gorak",
        text = {
            { line = "I’m all for a good fight, but lately, I've been taking hits like a paper bag in a rainstorm."},
            { line = "Got anything that'll make me... a little harder to squish?" , orderdescription = "Make Gorak harder to squish.", customerorder = "Damage Resistant Potion"},
        }
    },

    node4 = {
        character = "Gorak",
        text = {
            { line = "You know, I’ve been bumped, bruised, and... well, disintegrated a few too many times recently."},
            { line = "Got anything that’ll stop me from feeling like a piñata in a storm?" , orderdescription = "Make Gorak not be a piñata in a storm", customerorder = "Damage Resistant Potion"},
        }
    }
}

function GetDialogueNode(node)
    print("Getting node: " .. node)
    return dialogue[node]
end