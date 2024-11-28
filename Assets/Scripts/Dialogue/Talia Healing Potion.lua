dialogue = {
    node1 = {
        character = "Talia",
        text = {
            { line = "Alright, so I may or may not have ‘accidentally’ tried to outwrestle an iron bar."},
            { line = "Got something for... regrettable life choices?" , orderdescription = "Something for regrettable life choices", customerorder = "Healing Potion"},
        }
    },

    node2 = {
        character = "Talia",
        text = {
            { line = "Hot metal? Check. Flying sparks? Check. Burnt fingers? Double-check."},
            { line = "Got a potion to undo my bad aim?" , orderdescription = "Undo Talia's bad aim.", customerorder = "Healing Potion"},
        }
    },

    node3 = {
        character = "Talia",
        text = {
            { line = "Hammering away for hours is great... until your arm feels like it’s going to fall off."},
            { line = "What’ve you got for that?" , orderdescription = "For arms almost falling off", customerorder = "Healing Potion"},
        }
    },

    node4 = {
        character = "Talia",
        text = {
            { line = "Let’s just say I’ve been a little too enthusiastic at the forge today."},
            { line = "Got anything to fix, uh, enthusiasm?" , orderdescription = "A fix for Talia's enthusiasm", customerorder = "Healing Potion"},
        }
    }
}

function GetDialogueNode(node)
    print("Getting node: " .. node)
    return dialogue[node]
end