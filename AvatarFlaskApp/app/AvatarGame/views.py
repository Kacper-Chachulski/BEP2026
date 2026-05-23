import datetime
from flask import Blueprint, render_template, jsonify, session, redirect
from BOFS.util import *
from BOFS.globals import db
from BOFS.admin.util import verify_admin

# The name of this variable must match the folder's name.
AvatarGame = Blueprint('AvatarGame', __name__,
                          static_url_path='/AvatarGame',
                          template_folder='templates',
                          static_folder='static')


def handle_game_post():
    if "timePlayed" in request.form:
        logGameSum = db.GameSummaryLog()

        logGameSum.participantID = session['participantID']
        logGameSum.condition = session['condition']
        logGameSum.segment = request.form['segment']
        logGameSum.coins = request.form['coins']
        logGameSum.barrierHits = request.form['barrierHits']
        logGameSum.laneChanges = request.form['laneChanges']
        logGameSum.timePlayed = request.form['timePlayed']
        db.session.add(logGameSum)
    elif 'eventKey' in request.form:
        logGameKey= db.GameKeyLog()

        logGameKey.participantID = session['participantID']
        logGameKey.condition = session['condition']
        logGameKey.segment = request.form['segment']
        logGameKey.event = request.form['eventKey']
        db.session.add(logGameKey)
    elif 'key' in request.form:
        logNMKey= db.NMKeyLog()

        logNMKey.participantID = session['participantID']
        logNMKey.condition = session['condition']
        logNMKey.segment = request.form['segment']
        logNMKey.key = request.form['key']
        db.session.add(logNMKey)
        
    elif 'leftChoice' in request.form:
        logQuestKey= db.QuestKeyLog()

        logQuestKey.participantID = session['participantID']
        logQuestKey.choiceLeft = request.form['leftChoice']
        logQuestKey.choiceRight = request.form['rightChoice']
        logQuestKey.event = request.form['event']
        logQuestKey.timeScreen = request.form['timeSpent']
        db.session.add(logQuestKey)

    elif 'age' in request.form:
        logAvatar = db.AvatarLog()
        logAvatar.participantID = session['participantID']
        logAvatar.condition = session['condition']
        logAvatar.segment = request.form['segmentCI']
        logAvatar.age = request.form['age']
        logAvatar.gender = request.form['gender']
        logAvatar.weight = request.form['weight']
        logAvatar.skinTone = request.form['skinTone']
        logAvatar.height = request.form['height']
        db.session.add(logAvatar)

    elif 'exactTime' in request.form:
        logCustomizer = db.CustomizerLog()
        logCustomizer.participantID = session['participantID']
        logCustomizer.condition = session['condition']
        logCustomizer.segment = request.form['segmentCI']
        logCustomizer.attribute = request.form['attribute']
        logCustomizer.event = request.form['event']
        logCustomizer.position = request.form['position']
        logCustomizer.exactTime = request.form['exactTime']
        db.session.add(logCustomizer)

    elif 'keyTar' in request.form:
        logAttribute = db.AttributeLog()
        logAttribute.participantID = session['participantID']
        logAttribute.condition = session['condition']
        logAttribute.segment = request.form['segmentCI']
        logAttribute.attribute = request.form['attribute']
        logAttribute.startAttribute = request.form['startAttribute']
        logAttribute.midAttribute = request.form['midAttribute']
        logAttribute.endAttribute = request.form['tarAttribute']
        logAttribute.achievedMidPoint = request.form['achMid']
        logAttribute.achievedEndPoint = request.form['achTar']
        logAttribute.checkedNoChange = request.form['checkedNoChange']
        logAttribute.keyPressesToMid = request.form['keyMid']
        logAttribute.keyPressesToEnd = request.form['keyTar']
        logAttribute.timeSpent = request.form['timeSpentAtt']

        db.session.add(logAttribute)

    elif 'maleWeight' in request.form:
        logQuestWeights = db.QuestWeights()
        logQuestWeights.participantID = session['participantID']
        logQuestWeights.skinFair = request.form['fairWeight']
        logQuestWeights.skinDark = request.form['darkWeight']
        logQuestWeights.skinBrown = request.form['brownWeight']

        logQuestWeights.heightAverage = request.form['averageHeightWeight']
        logQuestWeights.heightShort = request.form['shortWeight']
        logQuestWeights.heightTall = request.form['tallWeight']

        logQuestWeights.ageAdult = request.form['adultWeight']
        logQuestWeights.ageOld = request.form['oldWeight']
        logQuestWeights.ageYoung = request.form['youngWeight']

        logQuestWeights.genderMale = request.form['maleWeight']
        logQuestWeights.genderFemale = request.form['femaleWeight']
        logQuestWeights.genderFluid = request.form['fluidWeight']

        logQuestWeights.weightAverage = request.form['averageWeightWeight']
        logQuestWeights.weightLarge = request.form['overweightWeight']
        logQuestWeights.weightFit = request.form['fitWeight']
        db.session.add(logQuestWeights)

    db.session.commit()
    return ""


@AvatarGame.route("/game_embed", methods=['POST', 'GET'])
@verify_correct_page
@verify_session_valid
def game_embed():
    if request.method == 'POST':
        return handle_game_post()
    return render_template("game_embed.html")


@AvatarGame.route("/game_fullscreen", methods=['POST', 'GET'])
@verify_correct_page
@verify_session_valid
def game_fullscreen():
    if request.method == 'POST':
        return handle_game_post()
    return render_template("game_fullscreen.html")


@AvatarGame.route("/game_custom", methods=['POST', 'GET'])
@verify_correct_page
@verify_session_valid
def game_custom():
    if request.method == 'POST':
        return handle_game_post()
    return render_template("game_custom.html")


@AvatarGame.route("/fetch_condition")
@verify_session_valid
def fetch_condition():
    return str(session['condition'])


@AvatarGame.route("/reset_session")
def reset_session():
    """Clear the current Flask session and redirect to the experiment start.

    Use this to ensure each run starts at the first PAGE_LIST entry (consent).
    """
    session.clear()
    return redirect('/')
