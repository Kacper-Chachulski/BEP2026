def create(db):
    class GameKeyLog(db.Model):
        __tablename__ = "game_key_log"
        gameLogID = db.Column(db.Integer, primary_key=True, autoincrement=True)
        participantID = db.Column(db.Integer, db.ForeignKey('participant.participantID'))
        condition = db.Column(db.Integer, db.ForeignKey('participant.condition'))
        segment = db.Column(db.Integer)
        event = db.Column(db.String)

        submittedOn = db.Column(db.DateTime, nullable=False, default=db.func.now())

    class NMKeyLog(db.Model):
        __tablename__ = "nm_key_log"
        gameLogID = db.Column(db.Integer, primary_key=True, autoincrement=True)
        participantID = db.Column(db.Integer, db.ForeignKey('participant.participantID'))
        condition = db.Column(db.Integer, db.ForeignKey('participant.condition'))
        segment = db.Column(db.Integer)
        key = db.Column(db.String)

        submittedOn = db.Column(db.DateTime, nullable=False, default=db.func.now())

    class QuestKeyLog(db.Model):
        __tablename__ = "quest_key_log"
        gameLogID = db.Column(db.Integer, primary_key=True, autoincrement=True)
        participantID = db.Column(db.Integer, db.ForeignKey('participant.participantID'))
        event = db.Column(db.String)
        choiceLeft = db.Column(db.String)
        choiceRight = db.Column(db.String)
        timeScreen = db.Column(db.String)

        submittedOn = db.Column(db.DateTime, nullable=False, default=db.func.now())

    class GameSummaryLog(db.Model):
        __tablename__ = "game_summary_log"
        gameLogID = db.Column(db.Integer, primary_key=True, autoincrement=True)
        participantID = db.Column(db.Integer, db.ForeignKey('participant.participantID'))
        condition = db.Column(db.Integer, db.ForeignKey('participant.condition'))
        segment = db.Column(db.Integer)
        coins = db.Column(db.Integer)
        barrierHits = db.Column(db.Integer)
        timePlayed = db.Column(db.String)
        laneChanges = db.Column(db.Integer)
        submittedOn = db.Column(db.DateTime, nullable=False, default=db.func.now())

    class AvatarLog(db.Model):
        __tablename__ = "avatar_summary_log"
        gameLogID = db.Column(db.Integer, primary_key=True, autoincrement=True)
        participantID = db.Column(db.Integer, db.ForeignKey('participant.participantID'))
        condition = db.Column(db.Integer, db.ForeignKey('participant.condition'))
        segment = db.Column(db.Integer)
        age = db.Column(db.String)
        gender = db.Column(db.String)
        weight = db.Column(db.String)
        skinTone = db.Column(db.String)
        height = db.Column(db.String)
        submittedOn = db.Column(db.DateTime, nullable=False, default=db.func.now())
    class CustomizerLog(db.Model):
        __tablename__ = "customizer_log"
        gameLogID = db.Column(db.Integer, primary_key=True, autoincrement=True)
        participantID = db.Column(db.Integer, db.ForeignKey('participant.participantID'))
        condition = db.Column(db.Integer, db.ForeignKey('participant.condition'))
        segment = db.Column(db.Integer)
        attribute = db.Column(db.String)
        event = db.Column(db.String)
        position = db.Column(db.String)
        exactTime = db.Column(db.String)

        submittedOn = db.Column(db.DateTime, nullable=False, default=db.func.now())

    class AttributeLog(db.Model):
        __tablename__ = "attribute_log"
        gameLogID = db.Column(db.Integer, primary_key=True, autoincrement=True)
        participantID = db.Column(db.Integer, db.ForeignKey('participant.participantID'))
        condition = db.Column(db.Integer, db.ForeignKey('participant.condition'))
        segment = db.Column(db.Integer)
        attribute = db.Column(db.String)
        startAttribute = db.Column(db.String)
        midAttribute = db.Column(db.String)
        endAttribute = db.Column(db.String)
        achievedMidPoint = db.Column(db.String)
        achievedEndPoint = db.Column(db.String)
        checkedNoChange = db.Column(db.String)
        keyPressesToMid = db.Column(db.Integer)
        keyPressesToEnd = db.Column(db.Integer)
        timeSpent = db.Column(db.String)

        
        submittedOn = db.Column(db.DateTime, nullable=False, default=db.func.now())

    class QuestWeights(db.Model):
        __tablename__ = "questWeights"
        gameLogID = db.Column(db.Integer, primary_key=True, autoincrement=True)
        participantID = db.Column(db.Integer, db.ForeignKey('participant.participantID'))

        skinFair = db.Column(db.Integer)
        skinDark = db.Column(db.Integer)
        skinBrown = db.Column(db.Integer)

        heightAverage = db.Column(db.Integer)
        heightShort = db.Column(db.Integer)
        heightTall = db.Column(db.Integer)

        ageAdult = db.Column(db.Integer)
        ageOld = db.Column(db.Integer)
        ageYoung = db.Column(db.Integer)

        genderMale = db.Column(db.Integer)
        genderFemale = db.Column(db.Integer)
        genderFluid = db.Column(db.Integer)

        weightAverage = db.Column(db.Integer)
        weightLarge = db.Column(db.Integer)
        weightFit = db.Column(db.Integer)

        submittedOn = db.Column(db.DateTime, nullable=False, default=db.func.now())
    return GameSummaryLog, GameKeyLog, NMKeyLog, QuestKeyLog, AvatarLog, CustomizerLog, AttributeLog, QuestWeights
