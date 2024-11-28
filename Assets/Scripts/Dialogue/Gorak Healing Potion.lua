dialogue = {
    node1 = {
        character = "Gorak",
        text = {
            { line = "You know when you feel like you’ve been hit by a truck, but you don’t remember seeing a truck? That’s me right now."},
            { line = "Something to fix the not-quite-dead-but-not-feeling-great vibe would be nice." , orderdescription = "Remove the not-feeling-great vibe.", customerorder = "Healing Potion"},
        }
    },

    node2 = {
        character = "Gorak",
        text = {
            { line = "I'm not too familiar with potions so I'm just going to tell you about what I need."},
            { line = "But I'll be guiding you by using magic, you'll see in a bit." , orderdescription = "Potion to relieve body pain.", customerorder = "Healing Potion"},
        }
    },

    node3 = {
        character = "Gorak",
        text = {
            { line = "You wouldn’t believe the week I’ve had. It's like my soul is buffering—stuck in a loop."},
            { line = "Got anything to reboot me?" , orderdescription = "Potion to reboot Gorak.", customerorder = "Healing Potion"},
        }
    },

    node4 = {
        character = "Gorak",
        text = {
            { line = "Sooo, do you have anything that could stop me from feeling like a burnt-out lantern?"},
            { line = "I could really use a little... revival." , orderdescription = "Revive Gorak for a little.", customerorder = "Healing Potion"},
        }
    }
}

function GetDialogueNode(node)
    print("Getting node: " .. node)
    return dialogue[node]
end